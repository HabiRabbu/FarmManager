using System;
using System.Collections;

using Harvey.Farm.Buildings;
using Harvey.Farm.Implements;
using Harvey.Farm.Jobs;
using Harvey.Farm.VehicleScripts;

using UnityEngine;

namespace Harvey.Farm.Jobs.VehicleField
{
    /// <summary>
    /// Detaches the implement and returns it to the nearest shed
    /// before the tractor is parked.
    /// </summary>
    public class ReturnImplementStep : IJobStep
    {
        readonly Func<Vehicle> _getVehicle;
        readonly string _toolId;     // id we fetched earlier
        Coroutine _co;
        bool _finished;

        public ReturnImplementStep(Func<Vehicle> getVeh, string toolId)
        {
            _getVehicle = getVeh ?? throw new ArgumentNullException(nameof(getVeh));
            _toolId = toolId;
        }

        public StepResult Tick(float dt)
        {
            if (string.IsNullOrEmpty(_toolId)) return StepResult.Done;

            var veh = _getVehicle();
            if (veh == null) return StepResult.Done;

            var implements = veh.GetComponent<ImplementHandler>();
            if (implements == null) return StepResult.Done;
            if (implements.CurrentImplement != null)
            {
                if (implements.CurrentImplement.GetGuid() != _toolId) return StepResult.Done;
            }

            if (_co == null)
            {
                var shed = BuildingManager.Instance.GetNearestShed(veh.transform.position);
                if (shed == null)
                {
                    Debug.LogWarning($"No shed to return implement {_toolId}");
                    return StepResult.Done;
                }

                _co = veh.StartCoroutine(ReturnRoutine(implements, shed));
            }

            Debug.Log($"ReturnImplementStep: Finished? {_finished}");
            return _finished ? StepResult.Done : StepResult.Running;
        }

        IEnumerator ReturnRoutine(ImplementHandler impl, ShedBuilding shed)
        {
            //yield return impl.Detach();      //Future anims potentiall?

            yield return impl.Return();

            _finished = true;
            _co = null;
        }
    }
}
