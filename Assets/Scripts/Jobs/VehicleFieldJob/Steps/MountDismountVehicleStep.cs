using System;

using Harvey.Farm.Jobs;
using Harvey.Farm.VehicleScripts;
using Harvey.Farm.Workers;

using UnityEngine;

namespace Harvey.Farm.Jobs.VehicleField
{
    /// <summary>
    /// Step for mounting a worker onto a vehicle.
    /// </summary>
    public class MountVehicleStep : IJobStep
    {
        readonly Worker _worker;
        readonly Func<Vehicle> _getVehicle;

        public MountVehicleStep(Worker worker, Func<Vehicle> getVehicle)
        {
            _worker = worker ?? throw new ArgumentNullException(nameof(worker));
            _getVehicle = getVehicle ?? throw new ArgumentNullException(nameof(getVehicle));
        }

        public StepResult Tick(float dt)
        {
            var vehicle = _getVehicle();
            if (vehicle == null) return StepResult.Running;

            try
            {
                Debug.Log($"Mounting worker '{_worker.name}' to vehicle '{vehicle.DisplayName}'");
                vehicle.Detach(); // Ensure vehicle is detached before mounting
                _worker.transform.SetParent(vehicle.transform, true);
                _worker.Meshes.SetActive(false);
                return StepResult.Done;
            }
            catch (Exception ex)
            {
                Debug.LogError($"MountVehicleStep failed: {ex.Message}");
                return StepResult.Failed;
            }
        }
    }

    /// <summary>
    /// Step for dismounting a worker from a vehicle.
    /// </summary>
    public class DismountVehicleStep : IJobStep
    {
        readonly Worker _worker;
        readonly Func<Vehicle> _getVehicle;

        public DismountVehicleStep(Worker worker, Func<Vehicle> getVehicle)
        {
            _worker = worker ?? throw new ArgumentNullException(nameof(worker));
            _getVehicle = getVehicle ?? throw new ArgumentNullException(nameof(getVehicle));
        }

        public StepResult Tick(float dt)
        {
            var vehicle = _getVehicle();
            if (vehicle == null) return StepResult.Done; // No vehicle to dismount from

            try
            {
                _worker.transform.SetParent(null, true);
                _worker.transform.position = vehicle.transform.position + Vector3.right;
                _worker.Meshes.SetActive(true);
                return StepResult.Done;
            }
            catch (Exception ex)
            {
                Debug.LogError($"DismountVehicleStep failed: {ex.Message}");
                return StepResult.Done; // Continue even if dismount fails
            }
        }
    }
}

