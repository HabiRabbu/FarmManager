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

        public ReserveVehicleStep(VehicleFieldJobInstance parent,
                                  string wantedVehicleId,
                                  Worker worker)
        {
            _parent = parent ?? throw new System.ArgumentNullException(nameof(parent));
            _wantedId = wantedVehicleId;
            _worker = worker ?? throw new System.ArgumentNullException(nameof(worker));
        }

        public bool Tick(float dt)
        {
            if (_parent.Vehicle != null)
            {
                if (!_parent.Vehicle.IsBusy)
                    _parent.Vehicle.SetBusy(true);
                return true;
            }

            // Find closest garage that owns an idle matching vehicle
            var garages = BuildingManager.Instance
                           .GetAllBuildings<GarageBuilding>()
                           .OrderBy(g => Vector3.Distance(_worker.transform.position, g.transform.position));

            foreach (var garage in garages)
            {
                if (garage.TryReserveVehicleById(_wantedId, out var reservedVehicle))
                {
                    _parent.Vehicle = reservedVehicle;
                    _parent.Garage = garage;
                    Debug.Log($"Reserved vehicle '{_wantedId}' from garage '{garage.name}' for worker '{_worker.name}'");
                    return true;
                }
            }
            Debug.LogWarning($"No available vehicle with ID '{_wantedId}' found in any garage.");
            return false;
        }
    }
}
