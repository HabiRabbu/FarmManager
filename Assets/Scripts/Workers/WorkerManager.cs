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
        [SerializeField] private int loadPriority = 6;
        public int LoadPriority => loadPriority;
        readonly List<Worker> workers = new();

        void Start()
        {
            SaveService.Instance.Register(this);
        }

        public void Register(Worker w) { if (!workers.Contains(w)) workers.Add(w); }
        public void Unregister(Worker w) { workers.Remove(w); }

        // Returns ALL idle workers that are not busy, regardless of scene
        public List<Worker> GetAllAvailable()
        {
            return workers.Where(w => w != null && !w.IsBusy).ToList();
        }

        public IEnumerable<Worker> AllWorkers
        {
            get
            {
                foreach (var w in workers)
                    if (w != null) yield return w;
            }
        }

        public Worker GetAvailable()
        {
            var firstAvailable = GetAllAvailable().FirstOrDefault();
            if (firstAvailable) return firstAvailable;

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