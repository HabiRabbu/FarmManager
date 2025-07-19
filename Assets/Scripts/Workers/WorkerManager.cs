using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Harvey.Data.Workers;
using Harvey.Farm.Buildings;
using Harvey.Farm.Events;
using Harvey.Farm.Factory;
using Harvey.SaveSystem;
using UnityEngine;

namespace Harvey.Farm.Workers
{
    public class WorkerManager : Singleton<WorkerManager>, ISaveSection
    {
        [SerializeField] public int LoadPriority { get; } = 6;
        readonly List<Worker> workers = new();

        void Start()
        {
            SaveService.Instance.Register(this);
        }

        public void Register(Worker w) { if (!workers.Contains(w)) workers.Add(w); }
        public void Unregister(Worker w) { workers.Remove(w); }

        // All workers that are idle *and* active in the scene
        public IEnumerable<Worker> SceneIdle =>
            workers.Where(w => !w.IsBusy && w.gameObject.activeSelf);

        // Returns ALL idle workers, including those in houses!
        public List<Worker> GetAllAvailable()
        {
            var list = new List<Worker>(SceneIdle);

            foreach (var h in BuildingManager.Instance.GetAllBuildings<HouseBuilding>())
                list.AddRange(h.GetIdleWorkers());

            return list;
        }

        public IEnumerable<Worker> AllWorkers
        {
            get
            {
                foreach (var w in workers)
                    if (w != null) yield return w;
            }
        }

        // Query scene first, then check houses
        public Worker GetAvailable()
        {
            var inScene = SceneIdle.FirstOrDefault();
            if (inScene) return inScene;

            foreach (var house in FindObjectsByType<HouseBuilding>(FindObjectsSortMode.None))
                if (house.TryGetIdleWorker(out var w))
                    return w;

            return null;
        }

        public Worker GetById(string id)
        {
            return workers.FirstOrDefault(w => w.GetId() == id);
        }

        /* --------------------- ISaveSection --------------------- */
        public void Capture(GameSaveData root)
        {
            // Capture all workers
            if (root.Workers == null)
                root.Workers = new WorkerSection();

            var sec = root.Workers;

            sec.Workers.Clear();

            foreach (var w in workers)
            {
                sec.Workers.Add(WorkerMapper.ToSaveData(w));
            }
        }

        public async Task Restore(GameSaveData root)
        {
            var rootWorkers = root.Workers;
            if (rootWorkers == null) return;

            GameEvents.StopAllTweens();

            /* A. despawn every live worker */
            foreach (var w in workers.ToArray())
            {
                WorkerFactory.Instance.Despawn(w.Model.PrefabGuid, w.gameObject);
            }
            workers.Clear();

            await RespawnWorkers(rootWorkers);

            Debug.Log($"WorkerManager: Restored W{workers.Count}");
        }

        private async Task RespawnWorkers(WorkerSection rootWorkers)
        {
            foreach (var workerData in rootWorkers.Workers)
            {
                var model = WorkerMapper.FromSaveData(workerData);
                var home = BuildingManager.Instance.GetById<HouseBuilding>(model.HomeId);

                var go = await WorkerFactory.Instance.SpawnAsync(model.PrefabGuid, null, model.LoadedPosition);
                if (go == null) continue;

                var goWorker = go.GetComponent<Worker>();

                goWorker.Stats.InitFromModel(model, home);

                home.TryAddOccupant(goWorker);

                if (!model.ActiveInScene)
                {
                    goWorker.ReturnHome();
                }
            }
        }
    }
}