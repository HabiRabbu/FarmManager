using System.Collections;
using Harvey.Farm.Buildings;
using Harvey.Farm.Implements;
using Harvey.Farm.VehicleScripts;
using UnityEngine;

public class ImplementHandler : MonoBehaviour
{
    [SerializeField] Transform hitchPoint;
    [SerializeField] ImplementLibrary library;

    ImplementBehaviour _currentImplement;

    public bool Has(JobType job) => _currentImplement && _currentImplement.Def.Job == job;

    public IEnumerator Fetch(JobType job, string toolId)
    {
        var shed = BuildingManager.Instance.GetNearestShed(transform.position);
        if (!shed) yield break;

        var mover = GetComponent<Mover>();
        yield return mover.MoveTo(shed.transform.position);

        ImplementBehaviour implement;
        shed.TryCheckoutByID(toolId, out implement);
        _currentImplement = implement;
        if (_currentImplement)
            _currentImplement?.AttachTo(hitchPoint);
        else
        {
            Debug.LogWarning($"<color=red>{name}</color> failed to fetch tool with ID: {toolId}");
            yield break;
        }
    }

    public IEnumerator Return()
    {
        if (!_currentImplement) yield break;
        var shed = BuildingManager.Instance.GetNearestShed(transform.position);
        yield return GetComponent<Mover>().MoveTo(shed.transform.position);
        shed.ReturnImplement(_currentImplement);
        _currentImplement = null;
    }
}
