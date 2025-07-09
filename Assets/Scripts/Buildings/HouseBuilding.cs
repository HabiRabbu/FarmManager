using System.Collections.Generic;
using System.Linq;
using Harvey.Farm.Events;
using Harvey.Farm.Factory;
using Harvey.Farm.Workers;
using Unity.VisualScripting;
using UnityEngine;

namespace Harvey.Farm.Buildings
{
    public class HouseBuilding : Building
    {
        [SerializeField] HouseDefinition houseDefinition;
        public override BuildingDefinition Definition => houseDefinition;
        public HouseDefinition HouseDefinition => houseDefinition;

        [SerializeField, Min(0f)] float spawnRadius = 1f;

        readonly List<Worker> occupants = new();
        readonly HashSet<Worker> idlePool = new();

        [SerializeField] Transform spawnPoint; // Anchor for worker spawning

        //Getters for UI and other systems
        public int Capacity => houseDefinition.Capacity;

        protected override void Start()
        {
            base.Start();

            if (houseDefinition != null)
                Debug.Log($"House initialised: {houseDefinition.DisplayName}");
            SpawnInitialWorkers();
        }

        // ------------ public API ----------------------------------------------

        public bool HasVacancy => occupants.Count < houseDefinition.Capacity;

        public bool TryAddOccupant(Worker w)
        {
            if (!HasVacancy) return false;
            occupants.Add(w);
            idlePool.Add(w);
            w.gameObject.SetActive(false);
            return true;
        }

        public bool TryGetIdleWorker(out Worker worker)
        {
            if (idlePool.Count > 0)
            {
                worker = idlePool.FirstOrDefault();

                idlePool.Remove(worker);
                DeployWorker(worker);
                return true;
            }
            worker = null;
            return false;
        }

        public IEnumerable<Worker> GetIdleWorkers() => idlePool;

        public void ReturnWorker(Worker w)
        {
            if (!idlePool.Add(w)) return;
            w.gameObject.SetActive(false);
            w.transform.SetParent(transform);
            GameEvents.BuildingStatsChanged();
        }

        // ---------- Internal ----------------------------------------------

        void SpawnInitialWorkers()
        {
            foreach (var def in houseDefinition.Preload)
            {
                var go = WorkerFactory.Instance.Spawn(def, spawnPoint, Vector3.zero);
                var worker = go.GetComponent<Worker>() ?? go.AddComponent<Worker>();

                worker.Init(def, this);

                TryAddOccupant(worker);
            }
        }

        public void DeployWorker(Worker w)
        {
            if (idlePool.Contains(w))
                idlePool.Remove(w);

            Vector2 offset = Random.insideUnitCircle * spawnRadius;
            Vector3 pos = (spawnPoint ? spawnPoint.position : transform.position) +
                          new Vector3(offset.x, 0f, offset.y);

            w.transform.SetParent(null);
            w.transform.position = pos;
            w.gameObject.SetActive(true);
        }

        // ---------- Radial Menu Support ----------
        public void RadialOpenBuildingInfo() => GameEvents.RadialBuildingInfoOpened(this);
        // -------------------------------------------
    }
}
