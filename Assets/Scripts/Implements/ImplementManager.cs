namespace Harvey.Farm.Implements
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Harvey.Data.Implements;
    using Harvey.Farm.Buildings;
    using Harvey.Farm.Factory;
    using Harvey.Farm.VehicleScripts;
    using Harvey.SaveSystem;
    using UnityEngine;

    public class ImplementManager : Singleton<ImplementManager>, ISaveSection
    {
        [SerializeField] public int LoadPriority { get; } = 5;

        [SerializeField] readonly List<ImplementBehaviour> plows = new();
        [SerializeField] readonly List<ImplementBehaviour> seeders = new();

        void Start()
        {
            SaveService.Instance.Register(this);
        }

        public void Register(ImplementBehaviour implementBehaviour)
        {
            Debug.Log($"Registering implement: {implementBehaviour.Model.Id} - GUID: {implementBehaviour.GetGuid()} - Type: {implementBehaviour.Model.Type}");

            if (implementBehaviour.Model.Type == ImplementType.Plow && !plows.Contains(implementBehaviour))
                plows.Add(implementBehaviour);
            else if (implementBehaviour.Model.Type == ImplementType.Seeder && !seeders.Contains(implementBehaviour))
                seeders.Add(implementBehaviour);

            Debug.Log($"After registration: P{plows.Count}/S{seeders.Count}");
        }

        public void Unregister(ImplementBehaviour implementBehaviour)
        {
            if (implementBehaviour.Model != null)
            {
                if (implementBehaviour.Model.Type == ImplementType.Plow) plows.Remove(implementBehaviour);
                if (implementBehaviour.Model.Type == ImplementType.Seeder) seeders.Remove(implementBehaviour);
            }
        }

        public ImplementBehaviour GetById(string id)
        {
            foreach (var p in plows)
                if (p.Model.Id == id) return p;

            foreach (var s in seeders)
                if (s.Model.Id == id) return s;

            Debug.LogWarning($"ImplementManager: no implement found with ID '{id}'");
            return null;
        }

        /* --------- Save Section ---------- */
        public void Capture(GameSaveData root)
        {
            // ensure the section exists
            if (root.Implements == null)
                root.Implements = new ImplementSection();

            var impl = root.Implements;

            impl.Plows.Clear();
            impl.Seeders.Clear();

            foreach (var p in plows)
                impl.Plows.Add(ImplementMappers.ToSaveData(p));

            foreach (var s in seeders)
                impl.Seeders.Add(ImplementMappers.ToSaveData(s));

            Debug.Log($"ImplementManager: captured P{impl.Plows.Count}/S{impl.Seeders.Count}");
        }

        public async Task Restore(GameSaveData root)
        {
            var impl = root.Implements;
            if (impl == null) return;

            Debug.Log($"BEFORE DESPAWN: P{plows.Count}/S{seeders.Count}");

            /* A. despawn every live implement */
            foreach (var p in plows.ToArray())
                ImplementFactory.Instance.Despawn(p.Model.PrefabGuid, p.gameObject);

            foreach (var s in seeders.ToArray())
                ImplementFactory.Instance.Despawn(s.Model.PrefabGuid, s.gameObject);

            plows.Clear();
            seeders.Clear();

            Debug.Log($"AFTER CLEAR: P{plows.Count}/S{seeders.Count}");

            /* B. respawn from save-data */
            await RespawnImplements(impl.Plows, plows);
            await RespawnImplements(impl.Seeders, seeders);

            Debug.Log($"ImplementManager: restored P{plows.Count}/S{seeders.Count}");
        }

        async Task RespawnImplements(IEnumerable<ImplementSaveData> dataList,
                       List<ImplementBehaviour> targetList)
        {
            foreach (var d in dataList)
            {
                var model = ImplementMappers.FromSaveData(d);

                ShedBuilding shed = (ShedBuilding)BuildingManager.Instance.GetById<ShedBuilding>(model.CurrentParentId);
                Vehicle vehicle = VehicleManager.Instance.GetById(model.CurrentParentId);

                Transform parentXform = shed ? shed.transform
                                      : vehicle ? vehicle.transform
                                                    : null;

                var go = await ImplementFactory.Instance.SpawnAsync(model.PrefabGuid, parentXform, model.LoadedPosition);
                if (go == null) continue;
                var beh = go.GetComponent<ImplementBehaviour>();

                var home = (ShedBuilding)BuildingManager.Instance.GetById<ShedBuilding>(model.HomeId);
                beh.InitFromModel(model, home);

                if (vehicle != null)
                {
                    var handler = vehicle.GetComponentInChildren<ImplementHandler>();
                    if (handler?.hitchPoint != null)
                    {
                        beh.AttachTo(handler.hitchPoint);
                    }
                    else
                        Debug.LogWarning($"Vehicle '{vehicle.name}' lacks ImplementHandler/hitchPoint");
                }
                else if (shed != null)
                {
                    shed.RegisterImplement(beh);
                    beh.Detach();
                    var anchor = shed.GetAnchor(model.AnchorIndex);
                    beh.AttachTo(anchor);
                }
            }
        }
    }
}