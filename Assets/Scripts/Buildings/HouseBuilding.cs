using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        readonly List<Worker> occupants = new();
        readonly HashSet<Worker> idlePool = new();

        [SerializeField] Transform spawnPoint;

        public HouseModel HouseModel => Model as HouseModel;

        public void InitFromModel(BuildingModel model)
        {
            occupants.Clear();
            idlePool.Clear();
            
            SetModel(model);
            SetId(model.Id);
            BuildingManager.Instance.Register(this);
        }

        async void Start()
        {
            if (houseDefinition != null)
            {
                SetModel(BuildingMapper.FromDefinition<HouseModel>(houseDefinition, GetId()));
                BuildingManager.Instance.Register(this);
                Debug.Log($"House initialised: {houseDefinition.DisplayName}");

                await SpawnInitialWorkers();
            }
        }

        // ------------ public API ----------------------------------------------

        public bool HasVacancy => occupants.Count < HouseModel.Capacity;

        public bool TryAddOccupant(Worker w)
        {
            if (!HasVacancy) return false;
            occupants.Add(w);
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
            w.transform.SetParent(transform);
            w.gameObject.SetActive(false);
            GameEvents.BuildingStatsChanged();
        }

        // ---------- Internal ----------------------------------------------

        async Task SpawnInitialWorkers()
        {
            foreach (var def in houseDefinition.Preload)
            {
                var go = await WorkerFactory.Instance.SpawnAsync(def.PrefabGuid, spawnPoint, Vector3.zero);
                var worker = go.GetComponent<Worker>() ?? go.AddComponent<Worker>();

                worker.Stats.InitFromModel(WorkerMapper.FromDefinition(def, this, worker.GetId()), this);

                TryAddOccupant(worker);
                ReturnWorker(worker);
            }
        }

        public void DeployWorker(Worker w)
        {
            if (idlePool.Contains(w))
                idlePool.Remove(w);

            Vector2 offset = Random.insideUnitCircle * HouseModel.SpawnRadius;
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
