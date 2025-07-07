using System.Collections.Generic;
using System.Linq;
using Harvey.Farm.Buildings;
using UnityEngine;

namespace Harvey.Farm.Workers
{
    public class WorkerManager : Singleton<WorkerManager>
    {
        readonly List<Worker> workers = new();

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
    }
}