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

        public StepResult Tick(float dt)
        {
            var vehicle = _getVehicle();
            if (vehicle == null) return StepResult.Running;

            var implementHandler = vehicle.GetComponent<ImplementHandler>();
            if (implementHandler == null)
            {
                Debug.LogWarning($"Vehicle {vehicle.name} is missing ImplementHandler component");
                return StepResult.Done; // Skip this step
            }

            // 1. Harvest jobs need no implement
            if (_jobType == JobType.Harvest) return StepResult.Done;

            // 2. Already has correct tool?
            if (implementHandler.CurrentImplement != null)
            {
                if (implementHandler.CurrentImplement.GetGuid() == _wantedImplId) return StepResult.Done;
            }

            // 3. If already fetching, just wait
            if (_runningCoroutine != null)
                return StepResult.Running;

            // 4. Find shed and check implement availability
            var shed = BuildingManager.Instance.GetNearestShed(vehicle.transform.position);
            if (shed == null)
            {
                Debug.LogError($"No shed found near vehicle '{vehicle.name}' to get implement!");
                return StepResult.Failed;
            }

            // Check if the implement exists in the shed
            if (!shed.HasImplement(_wantedImplId))
            {
                Debug.LogError($"Implement '{_wantedImplId}' does not exist in any shed.");
                return StepResult.Failed;
            }

            // Try to reserve the implement
            bool success = shed.ReserveToolById(_wantedImplId, out ImplementBehaviour implement);
            if (!success || implement == null)
            {
                // Implement exists but is currently in use - wait for it
                Debug.Log($"Implement '{_wantedImplId}' is busy. Waiting for resources...");
                return StepResult.WaitForResources;
            }

            // Successfully reserved - start fetching
            _runningCoroutine = vehicle.StartCoroutine(FetchAndClear(implementHandler, _wantedImplId));
            return StepResult.Running;
        }

        public void Cancel()
        {
            if (_runningCoroutine != null && _getVehicle() != null)
            {
                _getVehicle().StopCoroutine(_runningCoroutine);
                _runningCoroutine = null;
            }
        }

        IEnumerator FetchAndClear(ImplementHandler handler, string toolId)
        {
            yield return handler.Fetch(_jobType, toolId);

            _runningCoroutine = null;
        }
    }
}