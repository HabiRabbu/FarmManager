using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Harvey.Farm.Events;
using Harvey.Farm.Factory;
using Harvey.Farm.VehicleScripts;
using System.Threading.Tasks;

namespace Harvey.Farm.Buildings
{
    public class GarageBuilding : Building
    {
        [Header("Definition")]
        [SerializeField] GarageDefinition garageDefinition;
        public override BuildingDefinition Definition => garageDefinition;

        [Header("Vehicle Anchors")]
        [SerializeField] Transform[] parkingAnchors;

        readonly Dictionary<string, Vehicle> stock = new();
        readonly HashSet<string> reservedIds = new();

        VehicleDefinition[] preload;

        public GarageModel GarageModel => Model as GarageModel;


        public void InitFromModel(BuildingModel model)
        {
            stock.Clear();
            reservedIds.Clear();
            
            SetModel(model);
            SetId(model.Id);
            BuildingManager.Instance.Register(this);
        }

        async void Start()
        {
            if (garageDefinition != null)
            {
                preload = garageDefinition.Preload;

                SetModel(BuildingMapper.FromDefinition<GarageModel>(garageDefinition, GetId()));
                BuildingManager.Instance.Register(this);
                Debug.Log($"Garage initialised: {garageDefinition.DisplayName}");

                await SpawnInitialVehicles();
            }
        }

        /* ---------- public API ---------- */

        public bool Reserve(string id) => stock.ContainsKey(id) && reservedIds.Add(id);
        public void Unreserve(string id) => reservedIds.Remove(id);
        public bool IsReserved(string id) => reservedIds.Contains(id);

        public IEnumerable<Vehicle> Query(System.Func<Vehicle, bool> predicate) =>
            stock.Values.Where(v => !reservedIds.Contains(v._stats.GetId()) && predicate(v));

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
            reservedIds.Remove(v._stats.GetId());
            stock[v._stats.GetId()] = v;

            v.Detach();
            var anchor = GetFreeAnchor();
            v.AttachTo(anchor);
            GameEvents.BuildingStatsChanged();
        }

        /* ---------- internal ---------- */

        async Task SpawnInitialVehicles()
        {
            for (int i = 0; i < Mathf.Min(parkingAnchors.Length, preload.Length); i++)
            {
                var def = preload[i];
                var g = await VehicleFactory.Instance.SpawnAsync(def.PrefabGuid, parkingAnchors[i], Vector3.zero);
                var v = g.GetComponent<Vehicle>();

                if (v is Tractor)
                    v._stats.InitFromModel(VehicleMapper.FromDefinition<TractorModel>(def, this, v._stats.GetId()) as TractorModel, this);
                if (v is CombineHarvester)
                    v._stats.InitFromModel(VehicleMapper.FromDefinition<HarvesterModel>(def, this, v._stats.GetId()) as HarvesterModel, this);

                ReturnVehicle(v);
                Debug.Log($"Spawned initial vehicle: {v.name} at anchor {parkingAnchors[i].name}");
            }
        }

        public Transform GetFreeAnchor()
        {
            foreach (var a in parkingAnchors)
                if (a.childCount == 0) return a;
            return transform;
        }

        /* ---------- Radial menu ---------- */
        public void RadialOpenBuildingInfo() => GameEvents.RadialBuildingInfoOpened(this);
    }
}
