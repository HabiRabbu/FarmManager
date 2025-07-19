using System.Collections;
using Harvey.Farm.Buildings;
using Harvey.Farm.Implements;
using Harvey.Farm.Movement;
using Harvey.Farm.VehicleScripts;
using UnityEngine;

public class ImplementHandler : MonoBehaviour
{
    [SerializeField] public Transform hitchPoint;

    [SerializeField] public ImplementBehaviour _currentImplement;

    public bool Has(JobType job) 
    {
        bool hasImplement = _currentImplement && _currentImplement.gameObject && _currentImplement.Model.Job == job;
        Debug.Log($"ImplementHandler.Has({job}): _currentImplement={_currentImplement?.name ?? "null"}, hasCorrectJob={hasImplement}");
        return hasImplement;
    }

    public IEnumerator Fetch(JobType job, string toolId)
    {
        Debug.Log($"ImplementHandler.Fetch: Attempting to fetch {job} tool with ID {toolId}");
        
        var shed = BuildingManager.Instance.GetNearestShed(transform.position);
        if (!shed) 
        {
            Debug.LogWarning("No shed found near tractor!");
            yield break;
        }

        var mover = GetComponent<TractorMover>();
        yield return mover.MoveTo(shed.transform.position);

        ImplementBehaviour implement;
        bool success = shed.TryCheckoutByID(toolId, out implement);
        Debug.Log($"TryCheckoutByID result: {success}, implement: {implement?.name ?? "null"}");
        
        _currentImplement = implement;
        if (_currentImplement)
        {
            _currentImplement?.AttachTo(hitchPoint);
            // Update the tractor's AttachedToolId
            var tractor = GetComponent<Tractor>();
            if (tractor != null)
            {
                tractor.TractorModel.AttachedToolId = toolId;
                Debug.Log($"Tractor {tractor.name} now has attached tool: {toolId}");
            }
        }
        else
        {
            Debug.LogWarning($"<color=red>{name}</color> failed to fetch tool with ID: {toolId}");
            yield break;
        }
    }

    public IEnumerator Return()
    {
        if (!_currentImplement) 
        {
            yield break;
        }
        
        var shed = BuildingManager.Instance.GetNearestShed(transform.position);
        yield return GetComponent<TractorMover>().MoveTo(shed.transform.position);
        shed.ReturnImplement(_currentImplement);
        _currentImplement = null;
    }
}
