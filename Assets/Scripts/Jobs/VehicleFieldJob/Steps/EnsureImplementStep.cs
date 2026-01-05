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
    /// Step to ensure the vehicle has the correct implement attached for the job type.
    /// </summary>
    public class EnsureImplementStep : IJobStep
    {
        readonly Func<Vehicle> _getVehicle;
        readonly JobType _jobType;
        readonly string _wantedImplId;
        Coroutine _runningCoroutine;

        public EnsureImplementStep(Func<Vehicle> getVehicle, string implementId, JobType jobType)
        {
            _getVehicle = getVehicle ?? throw new ArgumentNullException(nameof(getVehicle));
            _jobType = jobType;
            _wantedImplId = implementId;
        }

        public bool Tick(float dt)
        {
            var vehicle = _getVehicle();
            if (vehicle == null) return false;

            var implementHandler = vehicle.GetComponent<ImplementHandler>();
            if (implementHandler == null)
            {
                Debug.LogWarning($"Vehicle {vehicle.name} is missing ImplementHandler component");
                return true; // Skip this step
            }

            // 1. Harvest jobs need no implement
            if (_jobType == JobType.Harvest) return true;

            // 2. Already has correct tool ?
            if (implementHandler.CurrentImplement != null)
            {
                if (implementHandler.CurrentImplement.GetGuid() == _wantedImplId) return true;
            }

            // 3. If already fetching, just return - Otherwise go get!
            if (_runningCoroutine == null)
            {
                var shed = BuildingManager.Instance.GetNearestShed(vehicle.transform.position);
                if (shed == null)
                {
                    Debug.LogError($"No shed found near vehicle '{vehicle.name}' to get implement!");
                    return false;
                }
                bool success = shed.ReserveToolById(_wantedImplId, out ImplementBehaviour implement);
                if (!success || implement == null)
                {
                    Debug.LogWarning($"Failed to reserve implement with ID '{_wantedImplId}' from shed near vehicle '{vehicle.name}'");
                    return false; // Cannot proceed without the implement
                }
                _runningCoroutine = vehicle.StartCoroutine(FetchAndClear(implementHandler, _wantedImplId));
            }
            return _runningCoroutine == null;
        }

        IEnumerator FetchAndClear(ImplementHandler handler, string toolId)
        {
            yield return handler.Fetch(_jobType, toolId);

            _runningCoroutine = null;
        }
    }
}