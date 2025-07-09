using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Harvey.Farm.Events;
using Harvey.Farm.Factory;
using Harvey.Farm.VehicleScripts;

namespace Harvey.Farm.Buildings
{
    public class GarageBuilding : Building
    {
        [Header("Definition")]
        [SerializeField] GarageDefinition garageDefinition;
        public override BuildingDefinition Definition => garageDefinition;
        public GarageDefinition GarageDefinition => garageDefinition;

        [Header("Vehicle Anchors")]
        [SerializeField] Transform[] parkingAnchors;

        readonly Dictionary<string, Vehicle> stock = new();
        readonly HashSet<string> reservedIds = new();

        VehicleDefinition[] preload;

        //Getters for UI and other systems
        public int Capacity => garageDefinition.VehicleSlots;

        void Awake() => preload = garageDefinition.Preload;

        protected override void Start()
        {
            base.Start();
            Debug.Log($"Tractor garage initialised: {DisplayName}");
            SpawnInitialVehicles();
        }

        /* ---------- public API ---------- */

        public bool Reserve(string id) => stock.ContainsKey(id) && reservedIds.Add(id);
        public void Unreserve(string id) => reservedIds.Remove(id);
        public bool IsReserved(string id) => reservedIds.Contains(id);

        public IEnumerable<Vehicle> Query(System.Func<Vehicle, bool> predicate) =>
            stock.Values.Where(v => !reservedIds.Contains(v.Id) && predicate(v));

        public bool TryCheckout(string id, out Vehicle vehicle)
        {
            if (stock.TryGetValue(id, out vehicle) && reservedIds.Contains(id))
            {
                reservedIds.Remove(id);
                stock.Remove(id);
                GameEvents.BuildingStatsChanged();
                return true;
            }
            vehicle = null;
            return false;
        }

        public void ReturnVehicle(Vehicle v)
        {
            reservedIds.Remove(v.Id);
            stock[v.Id] = v;

            v.Detach();
            var anchor = GetFreeAnchor();
            v.AttachTo(anchor);
            GameEvents.BuildingStatsChanged();
        }

        /* ---------- internal ---------- */

        void SpawnInitialVehicles()
        {
            for (int i = 0; i < Mathf.Min(parkingAnchors.Length, preload.Length); i++)
            {
                var def = preload[i];
                var g = VehicleFactory.Instance.Spawn(def, parkingAnchors[i], Vector3.zero);
                var v = g.GetComponent<Vehicle>();

                v._stats.Init(def, this);

                ReturnVehicle(v);
            }
        }

        Transform GetFreeAnchor()
        {
            foreach (var a in parkingAnchors)
                if (a.childCount == 0) return a;
            return transform;
        }

        /* ---------- Radial menu ---------- */
        public void RadialOpenBuildingInfo() => GameEvents.RadialBuildingInfoOpened(this);
    }
}
