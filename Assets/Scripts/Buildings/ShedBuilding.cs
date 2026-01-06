using System.Collections.Generic;
using UnityEngine;
using Harvey.Farm.Implements;
using System.Linq;
using Harvey.Farm.Factory;
using System;
using Harvey.Farm.Events;
using NUnit.Framework.Internal;
using System.Threading.Tasks;

namespace Harvey.Farm.Buildings
{
    public class ShedBuilding : Building
    {

        [Header("Definition")]
        [SerializeField] private ShedDefinition shedDefinition;
        public override BuildingDefinition Definition => shedDefinition;

        [Header("Implement spawn anchors")]
        [SerializeField] Transform[] implantAnchors;

        [Header("Inventory")]
        [SerializeField] readonly Dictionary<string, ImplementBehaviour> stock = new();
        [SerializeField] readonly HashSet<string> reserved = new();

        ImplementDefinition[] preload;

        public ShedModel ShedModel => Model as ShedModel;

        public void InitFromModel(BuildingModel model)
        {
            stock.Clear();
            reserved.Clear();

            SetModel(model);
            SetId(model.Id);
            BuildingManager.Instance.Register(this);
        }

        async void Start()
        {
            if (shedDefinition != null)
            {
                preload = shedDefinition.Preload;

                SetModel(BuildingMapper.FromDefinition<ShedModel>(shedDefinition, GetId()));
                BuildingManager.Instance.Register(this);
                Debug.Log($"Shed initialised: {shedDefinition.DisplayName}");

                await TestingSpawnInitialImplements();
            }
        }

        // ---------- Public API ----------

        public bool Reserve(string id) => stock.ContainsKey(id) && reserved.Add(id);
        public void Unreserve(string id) => reserved.Remove(id);
        public bool IsReserved(string id) => reserved.Contains(id);
        public bool HasImplement(string id) => stock.ContainsKey(id);

        public IEnumerable<ImplementBehaviour> Query(System.Func<ImplementBehaviour, bool> predicate) =>
            stock.Values.Where(b => !reserved.Contains(b.Model.Id) && predicate(b));

        public bool ReserveToolById(string id, out ImplementBehaviour implement)
        {
            if (stock.TryGetValue(id, out implement) && !reserved.Contains(id))
            {
                reserved.Add(id);
                GameEvents.BuildingStatsChanged();
                return true;
            }
            implement = null;
            return false;
        }

        public bool TryCheckoutByID(string id, out ImplementBehaviour implement)
        {
            if (stock.TryGetValue(id, out implement) && reserved.Contains(id))
            {
                reserved.Remove(id);
                stock.Remove(id);

                GameEvents.BuildingStatsChanged();
                return true;
            }
            implement = null;
            return false;
        }

        public void RegisterImplement(ImplementBehaviour implement)
        {
            if (implement == null || stock.ContainsKey(implement.Model.Id)) return;

            stock[implement.Model.Id] = implement;
            GameEvents.BuildingStatsChanged();
        }

        public void ReturnImplement(ImplementBehaviour implement, int index = -1)
        {
            if (reserved.Contains(implement.Model.Id))
            {
                reserved.Remove(implement.Model.Id);
            }
            stock[implement.Model.Id] = implement;

            implement.Detach();
            var anchor = GetFreeAnchor();
            if (index >= 0 && index < implantAnchors.Length)
                anchor = GetAnchor(index);
            implement.AttachTo(anchor);

            GameEvents.BuildingStatsChanged();
            GameEvents.ResourcesAvailable();
        }

        // ---------- Internal ----------
        async Task TestingSpawnInitialImplements()
        {
            int i = 0;

            var def = preload[i];
            var go = await ImplementFactory.Instance.SpawnAsync(def.PrefabGuid, implantAnchors[i], Vector3.zero);
            var beh = go.GetComponent<ImplementBehaviour>() ?? go.AddComponent<ImplementBehaviour>();
            beh.InitFromModel(ImplementMappers.FromDefinition(def, this, beh.GetGuid()), this);
            ReturnImplement(beh);
            i++;

            def = preload[i];
            go = await ImplementFactory.Instance.SpawnAsync(def.PrefabGuid, implantAnchors[i], Vector3.zero);
            beh = go.GetComponent<ImplementBehaviour>() ?? go.AddComponent<ImplementBehaviour>();
            beh.InitFromModel(ImplementMappers.FromDefinition(def, this, beh.GetGuid()), this);
            ReturnImplement(beh);
            i++;

            def = preload[i];
            go = await ImplementFactory.Instance.SpawnAsync(def.PrefabGuid, implantAnchors[i], Vector3.zero);
            beh = go.GetComponent<ImplementBehaviour>() ?? go.AddComponent<ImplementBehaviour>();
            beh.InitFromModel(ImplementMappers.FromDefinition(def, this, beh.GetGuid()), this);
            ReturnImplement(beh);
        }

        public Transform GetFreeAnchor()
        {
            foreach (var anchor in implantAnchors)
                if (anchor.childCount == 0)
                    return anchor;
            return transform;
        }

        public Transform GetAnchor(int index) => (index >= 0 && index < implantAnchors.Length)
                                         ? implantAnchors[index] : transform;
        public int IndexOfAnchor(Transform t) => System.Array.IndexOf(implantAnchors, t);

        // ---------- Radial Menu Support ----------
        public void RadialOpenBuildingInfo() => GameEvents.RadialBuildingInfoOpened(this);
        // -------------------------------------------
    }
}
