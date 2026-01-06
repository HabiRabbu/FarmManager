using System.Linq;

using Harvey.Farm.Buildings;
using Harvey.Farm.Jobs;
using Harvey.Farm.VehicleScripts;
using Harvey.Farm.Workers;

using UnityEngine;

namespace Harvey.Farm.Jobs.VehicleField
{
    /// <summary>
    /// Step responsible for reserving an appropriate vehicle from the nearest garage.
    /// </summary>
    public class ReserveVehicleStep : IJobStep
    {
        readonly VehicleFieldJobInstance _parent;
        readonly string _wantedId;
        readonly Worker _worker;
        bool _vehicleExistsButBusy;

        public ReserveVehicleStep(VehicleFieldJobInstance parent,
                                  string wantedVehicleId,
                                  Worker worker)
        {
            _parent = parent ?? throw new System.ArgumentNullException(nameof(parent));
            _wantedId = wantedVehicleId;
            _worker = worker ?? throw new System.ArgumentNullException(nameof(worker));
        }

        public StepResult Tick(float dt)
        {
            if (_parent.Vehicle != null)
            {
                if (!_parent.Vehicle.IsBusy)
                    _parent.Vehicle.SetBusy(true);
                return StepResult.Done;
            }

            // Check if the vehicle exists anywhere first
            var allGarages = BuildingManager.Instance.GetAllBuildings<GarageBuilding>();
            bool vehicleExists = false;
            _vehicleExistsButBusy = false;

            // Find closest garage that owns an idle matching vehicle
            var garages = allGarages
                           .OrderBy(g => Vector3.Distance(_worker.transform.position, g.transform.position));

            foreach (var garage in garages)
            {
                // Check if this garage has the vehicle at all
                if (garage.HasVehicle(_wantedId))
                {
                    vehicleExists = true;

                    if (garage.TryReserveVehicleById(_wantedId, out var reservedVehicle))
                    {
                        _parent.Vehicle = reservedVehicle;
                        _parent.Garage = garage;
                        Debug.Log($"Reserved vehicle '{_wantedId}' from garage '{garage.name}' for worker '{_worker.name}'");
                        return StepResult.Done;
                    }
                    else
                    {
                        // Vehicle exists but is reserved/busy
                        _vehicleExistsButBusy = true;
                    }
                }
            }

            // Also check if the vehicle is out in the world (in use)
            var vehicleInWorld = VehicleManager.Instance.GetById(_wantedId);
            if (vehicleInWorld != null)
            {
                vehicleExists = true;
                if (vehicleInWorld.IsBusy)
                {
                    _vehicleExistsButBusy = true;
                }
            }

            if (!vehicleExists)
            {
                Debug.LogError($"Vehicle with ID '{_wantedId}' does not exist!");
                return StepResult.Failed;
            }

            if (_vehicleExistsButBusy)
            {
                Debug.Log($"Vehicle '{_wantedId}' exists but is currently busy. Waiting for resources...");
                return StepResult.WaitForResources;
            }

            Debug.LogWarning($"No available vehicle with ID '{_wantedId}' found in any garage.");
            return StepResult.WaitForResources;
        }
    }
}
