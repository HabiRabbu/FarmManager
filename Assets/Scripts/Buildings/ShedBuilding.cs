using System.Collections.Generic;
using UnityEngine;
using Harvey.Farm.Crops;
using Harvey.Farm.Implements;
using System.Linq;
using Harvey.Farm.Factory;
using System;
using Harvey.Farm.Events;

namespace Harvey.Farm.Buildings
{
    public class ShedBuilding : Building
    {
        [Header("Implement spawn anchors")]
        [SerializeField] Transform[] implantAnchors;
        [SerializeField] ImplementDefinition[] preload;

        [Header("Inventory")]
        [SerializeField] readonly Dictionary<string, ImplementBehaviour> stock = new();
        [SerializeField] readonly HashSet<string> reserved = new();

        void Start() => SpawnInitialImplements();


        // ---------- Public API ----------

        public bool Reserve(string id) => stock.ContainsKey(id) && reserved.Add(id);
        public void Unreserve(string id) => reserved.Remove(id);
        public bool IsReserved(string id) => reserved.Contains(id);

        public IEnumerable<ImplementBehaviour> Query(System.Func<ImplementBehaviour, bool> predicate) =>
            stock.Values.Where(b => !reserved.Contains(b.id) && predicate(b));

        public bool TryCheckoutByID(string id, out ImplementBehaviour implement)
        {
            if (stock.TryGetValue(id, out implement) && reserved.Contains(id))
            {
                reserved.Remove(id);
                stock.Remove(id);

                GameEvents.ShedInventoryChanged();
                return true;
            }
            implement = null;
            return false;
        }

        public void ReturnImplement(ImplementBehaviour implement)
        {
            reserved.Remove(implement.id);
            stock[implement.id] = implement;

            implement.Detach();
            var anchor = GetFreeAnchor();
            implement.AttachTo(anchor);

            GameEvents.ShedInventoryChanged();
        }

        // ---------- Internal ----------

        void SpawnInitialImplements()
        {
            for (int i = 0; i < Mathf.Min(implantAnchors.Length, preload.Length); i++)
            {
                var def = preload[i];
                var go = ImplementFactory.Instance.Spawn(def, implantAnchors[i], Vector3.zero);
                var beh = go.GetComponent<ImplementBehaviour>() ?? go.AddComponent<ImplementBehaviour>();
                beh.Init(def, this);
                ReturnImplement(beh);
            }
        }

        Transform GetFreeAnchor()
        {
            foreach (var anchor in implantAnchors)
                if (anchor.childCount == 0)
                    return anchor;
            return transform;
        }
    }
}
