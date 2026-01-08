using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using System.Threading.Tasks;
using Harvey.Data.Jobs;
using Harvey.SaveSystem;
using Harvey.Farm.Fields;
using Harvey.Data.Coffee;
using Harvey.Farm.Jobs;
using Harvey.Data.Vehicles;
using Harvey.Farm.Buildings;
using Harvey.Farm.Factory;
using UnityEngine;
using Harvey.Farm.Events;

namespace Harvey.Farm.VehicleScripts
{
    public class VehicleManager : Singleton<VehicleManager>, ISaveSection
    {
        [SerializeField] private int loadPriority = 4;
        public int LoadPriority => loadPriority;

        [SerializeField] readonly List<Vehicle> tractors = new();
        [SerializeField] readonly List<Vehicle> harvesters = new();

        public IEnumerable<Vehicle> IdleVehicles
        {
            get
            {
                foreach (var v in tractors)
                {
                    if (v == null) continue;
                    Debug.Log($"Checking Tractor: {v.name}, IsBusy: {v.IsBusy}, CanDo(Plow): {v.CanDo(JobType.Plow)}");
                    if (!v.IsBusy) yield return v;
                }

                foreach (var v in harvesters)
                {
                    if (v == null) continue;
                    Debug.Log($"Checking Harvester: {v.name}, IsBusy: {v.IsBusy}, CanDo(Harvest): {v.CanDo(JobType.Harvest)}");
                    if (!v.IsBusy) yield return v;
                }
            }
        }
        public IEnumerable<Vehicle> AllVehicles
        {
            get
            {
                foreach (var v in tractors)
                    if (v != null) yield return v;
                foreach (var v in harvesters)
                    if (v != null) yield return v;
            }
        }

        void Start()
        {
            SaveService.Instance.Register(this);
        }

        public void RegisterVehicle(Vehicle v)
        {
            if (v._stats.Model.Type == VehicleType.Tractor && !tractors.Contains(v))
            {
                Debug.Log($"Registering Tractor: {v.name}");
                tractors.Add(v);
            }
            else if (v._stats.Model.Type == VehicleType.CombineHarvester && !harvesters.Contains(v))
            {
                Debug.Log($"Registering Harvester: {v.name}");
                harvesters.Add(v);
            }
        }
        public void UnregisterVehicle(Vehicle v)
        {
            if (v is Tractor && tractors.Contains(v))
            {
                Debug.Log($"Unregistering Tractor: {v.name}");
                tractors.Remove(v);
            }
            else if (v is CombineHarvester && harvesters.Contains(v))
            {
                Debug.Log($"Unregistering Harvester: {v.name}");
                harvesters.Remove(v);
            }
        }

        public Vehicle GetById(string id)
        {
            //Find in tractors OR harvesters
            foreach (var v in tractors)
            {
                if (v._stats.GetId() == id) return v;
            }
            foreach (var v in harvesters)
            {
                if (v._stats.GetId() == id) return v;
            }
            Debug.LogWarning($"Vehicle with ID {id} not found.");
            return null;
        }

        /* --------------------- ISaveSection --------------------- */
        public void Capture(GameSaveData root)
        {
            // 1. save vehicles
            if (root.Vehicles == null)
                root.Vehicles = new VehicleSection();

            var sec = root.Vehicles;

            sec.Tractors.Clear();
            sec.Harvesters.Clear();

            foreach (var v in tractors)
                sec.Tractors.Add(VehicleMapper.ToSaveData(v as Tractor));
            foreach (var h in harvesters)
                sec.Harvesters.Add(VehicleMapper.ToSaveData(h as CombineHarvester));
        }

        public async Task Restore(GameSaveData root)
        {
            var vehicles = root.Vehicles;
            if (vehicles == null) return;

            GameEvents.StopAllTweens();

            /* A. despawn every live vehicle */
            foreach (var tractor in tractors.ToArray())
            {
                Debug.Log($"Despawning tractor: {tractor.DisplayName}");
                VehicleFactory.Instance.Despawn(tractor._stats.Model.PrefabGuid, tractor.gameObject);
            }
            foreach (var harvester in harvesters.ToArray())
            {
                Debug.Log($"Despawning harvester: {harvester.DisplayName}");
                VehicleFactory.Instance.Despawn(harvester._stats.Model.PrefabGuid, harvester.gameObject);
            }
            tractors.Clear();
            harvesters.Clear();

            /* B. respawn from save-data - WAIT for completion! */
            await RespawnTractorsAsync(vehicles.Tractors);
            await RespawnHarvestersAsync(vehicles.Harvesters);

            Debug.Log($"VehicleManager: restored T{tractors.Count}/H{harvesters.Count}");
        }

        async Task RespawnTractorsAsync(IEnumerable<TractorSaveData> dataList)
        {
            foreach (var data in dataList)
            {
                var model = VehicleMapper.FromSaveData(data);
                var home = BuildingManager.Instance.GetById<GarageBuilding>(model.HomeId);

                var go = await VehicleFactory.Instance.SpawnAsync(model.PrefabGuid, null, model.LoadedPosition);
                if (go == null) continue;
                go.transform.rotation = model.LoadedRotation;

                var tractor = go.GetComponent<Tractor>();
                tractor._stats.InitFromModel(model, home);

                RegisterVehicle(tractor);

                // Debug logging
                Debug.Log($"Respawned tractor: {tractor.DisplayName}, IsBusy: {tractor.IsBusy}, Model.IsBusy: {tractor._stats.Model.IsBusy}, CanDo(Plow): {tractor.CanDo(JobType.Plow)}");

                if (!tractor._stats.Model.IsBusy)
                    tractor.ReturnHome();
            }
        }

        async Task RespawnHarvestersAsync(IEnumerable<HarvesterSaveData> dataList)
        {
            foreach (var data in dataList)
            {
                var model = VehicleMapper.FromSaveData(data);
                var home = BuildingManager.Instance.GetById<GarageBuilding>(model.HomeId);

                var go = await VehicleFactory.Instance.SpawnAsync(model.PrefabGuid, null, model.LoadedPosition);
                if (go == null) continue;
                go.transform.rotation = model.LoadedRotation;

                var harvester = go.GetComponent<CombineHarvester>();
                harvester._stats.InitFromModel(model, home);

                RegisterVehicle(harvester);

                // Debug logging
                Debug.Log($"Respawned harvester: {harvester.DisplayName}, IsBusy: {harvester.IsBusy}, Model.IsBusy: {harvester._stats.Model.IsBusy}, CanDo(Harvest): {harvester.CanDo(JobType.Harvest)}");

                if (!harvester._stats.Model.IsBusy)
                    harvester.ReturnHome();
            }
        }
    }
}
