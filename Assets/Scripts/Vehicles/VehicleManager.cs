using System.Collections.Generic;
using UnityEngine;

namespace Harvey.Farm.VehicleScripts
{
    public class VehicleManager : Singleton<VehicleManager>
    {
        private List<Vehicle> vehicles = new();

        public IReadOnlyList<Vehicle> AllVehicles => vehicles;

        public IEnumerable<Vehicle> IdleVehicles
        {
            get
            {
                foreach (var v in vehicles)
                    if (!v.IsBusy) yield return v;
            }
        }

        public void RegisterVehicle(Vehicle v)
        {
            if (!vehicles.Contains(v))
            {
                Debug.Log($"Registering: {v.name}");
                vehicles.Add(v);
            }
        }
        public void UnregisterVehicle(Vehicle v)
        {
            if (vehicles.Contains(v))
                vehicles.Remove(v);
        }

        public Vehicle GetAvailableVehicle()
        {
            return vehicles.Find(v => !v.Stats.IsBusy);
        }

        public T GetAvailableVehicle<T>() where T : Vehicle
        {
            return vehicles.Find(v => v is T && !v.Stats.IsBusy) as T;
        }
    }
}
