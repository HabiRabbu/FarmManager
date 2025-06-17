using System.Collections.Generic;
using UnityEngine;

namespace Harvey.Farm.VehicleScripts
{
    public class VehicleManager : MonoBehaviour
    {
        public static VehicleManager Instance { get; private set; }

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

        void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }

        public void RegisterVehicle(Vehicle v)
        {
            if (!vehicles.Contains(v))
                vehicles.Add(v);
        }
        public void UnregisterVehicle(Vehicle v)
        {
            if (vehicles.Contains(v))
                vehicles.Remove(v);
        }

        public Vehicle GetAvailableVehicle()
        {
            return vehicles.Find(v => !v.IsBusy);
        }

        public T GetAvailableVehicle<T>() where T : Vehicle
        {
            return vehicles.Find(v => v is T && !v.IsBusy) as T;
        }
    }
}
