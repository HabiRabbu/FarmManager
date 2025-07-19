using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Harvey.Data.Buildings;
using Harvey.Farm.Factory;
using Harvey.SaveSystem;
using UnityEngine;

namespace Harvey.Farm.Buildings
{
    public class BuildingManager : Singleton<BuildingManager>, ISaveSection
    {
        [SerializeField] public int LoadPriority { get; } = 3;

        [Header("Scene containers")]
        [SerializeField] Transform shedParent;
        [SerializeField] Transform garageParent;
        [SerializeField] Transform houseParent;

        [SerializeField] readonly List<ShedBuilding> sheds = new();
        [SerializeField] readonly List<HouseBuilding> houses = new();
        [SerializeField] readonly List<GarageBuilding> garages = new();

        void Start()
        {
            SaveService.Instance.Register(this);
        }

        public void Register(Building b)
        {
            if (b is ShedBuilding shed && !sheds.Contains(shed))
                sheds.Add(shed);
            if (b is HouseBuilding house && !houses.Contains(house))
                houses.Add(house);
            if (b is GarageBuilding garage && !garages.Contains(garage))
                garages.Add(garage);
        }
        public void Unregister(Building b)
        {
            if (b is ShedBuilding shed) sheds.Remove(shed);
            if (b is HouseBuilding house) houses.Remove(house);
            if (b is GarageBuilding garage) garages.Remove(garage);
        }

        public ShedBuilding GetNearestShed(Vector3 point)
        {
            ShedBuilding best = null;
            float bestSqr = float.MaxValue;
            foreach (var s in sheds)
            {
                float d = (s.transform.position - point).sqrMagnitude;
                if (d < bestSqr) { best = s; bestSqr = d; }
            }
            return best;
        }

        public IEnumerable<T> GetAllBuildings<T>() where T : Building
        {
            if (typeof(T) == typeof(ShedBuilding))
                return sheds.Cast<T>();
            if (typeof(T) == typeof(HouseBuilding))
                return houses.Cast<T>();

            return Enumerable.Empty<T>();
        }

        public T GetById<T>(string id) where T : Building
        {
            if (typeof(T) == typeof(ShedBuilding))
                return sheds.FirstOrDefault(s => s.GetId() == id) as T;
            if (typeof(T) == typeof(HouseBuilding))
                return houses.FirstOrDefault(h => h.GetId() == id) as T;
            if (typeof(T) == typeof(GarageBuilding))
                return garages.FirstOrDefault(g => g.GetId() == id) as T;

            return null;
        }

        /* ---------- Save Section ---------- */
        public void Capture(GameSaveData root)
        {
            // ensure the section exists
            if (root.Buildings == null)
                root.Buildings = new BuildingSection();

            var buildings = root.Buildings;

            buildings.Houses.Clear();
            buildings.Sheds.Clear();
            buildings.Garages.Clear();

            foreach (var house in houses)
                buildings.Houses.Add(BuildingMapper.ToSaveData(house.HouseModel, house.transform.position, house.transform.rotation));

            foreach (var shed in sheds)
                buildings.Sheds.Add(BuildingMapper.ToSaveData(shed.ShedModel, shed.transform.position, shed.transform.rotation));

            foreach (var garage in garages)
                buildings.Garages.Add(BuildingMapper.ToSaveData(garage.GarageModel, garage.transform.position, garage.transform.rotation));

            Debug.Log($"BuildingManager: captured H{buildings.Houses.Count}/S{buildings.Sheds.Count}/G{buildings.Garages.Count}");
        }

        public async Task Restore(GameSaveData root)
        {
            var buildings = root.Buildings;
            if (buildings == null) return;

            /* A. despawn every live building */
            foreach (var house in houses.ToArray())
            {
                Debug.Log($"Despawning house: {house.Model.DisplayName}");
                BuildingFactory.Instance.Despawn(house.Model.PrefabGuid, house.gameObject);
            }
            foreach (var shed in sheds.ToArray())
            {
                Debug.Log($"Despawning shed: {shed.Model.DisplayName}");
                BuildingFactory.Instance.Despawn(shed.Model.PrefabGuid, shed.gameObject);
            }
            foreach (var garage in garages.ToArray())
            {
                Debug.Log($"Despawning garage: {garage.Model.DisplayName}");
                BuildingFactory.Instance.Despawn(garage.Model.PrefabGuid, garage.gameObject);
            }

            houses.Clear();
            sheds.Clear();
            garages.Clear();

            /* B. respawn from save-data */
            await RespawnHouses(buildings.Houses);
            await RespawnSheds(buildings.Sheds);
            await RespawnGarages(buildings.Garages);

            Debug.Log($"BuildingManager: restored H{houses.Count}/S{sheds.Count}/G{garages.Count}");
        }

        async Task RespawnHouses(IEnumerable<HouseSaveData> dataList)
        {
            foreach (var data in dataList)
            {
                var model = BuildingMapper.FromSaveData(data);
                var go = await BuildingFactory.Instance.SpawnAsync(model.PrefabGuid, houseParent, model.LoadedPosition);
                if (go == null) continue;
                go.transform.rotation = model.LoadedRotation;
                var building = go.GetComponent<HouseBuilding>();
                building.InitFromModel(model);
                building.transform.SetParent(houseParent, false);
            }
        }

        async Task RespawnSheds(IEnumerable<ShedSaveData> dataList)
        {
            foreach (var data in dataList)
            {
                var model = BuildingMapper.FromSaveData(data);
                var go = await BuildingFactory.Instance.SpawnAsync(model.PrefabGuid, shedParent, model.LoadedPosition);
                if (go == null) continue;
                go.transform.rotation = model.LoadedRotation;
                var building = go.GetComponent<ShedBuilding>();
                building.InitFromModel(model);
                building.transform.SetParent(shedParent, false);
            }
        }

        async Task RespawnGarages(IEnumerable<GarageSaveData> dataList)
        {
            foreach (var data in dataList)
            {
                var model = BuildingMapper.FromSaveData(data);
                var go = await BuildingFactory.Instance.SpawnAsync(model.PrefabGuid, garageParent, model.LoadedPosition);
                if (go == null) continue;
                go.transform.rotation = model.LoadedRotation;
                var building = go.GetComponent<GarageBuilding>();
                building.InitFromModel(model);
                building.transform.SetParent(garageParent, false);
            }
        }
    }
}
