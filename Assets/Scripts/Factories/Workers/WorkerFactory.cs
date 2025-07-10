using UnityEngine;
using Harvey.Farm.Utilities;
using Harvey.Farm.Workers;  // PoolManager

namespace Harvey.Farm.Factory
{
    public class WorkerFactory : Singleton<WorkerFactory>, IWorkerFactory
    {
        /* -------- IWorkerFactory -------- */
        public GameObject Spawn(WorkerDefinition worker, Transform parent, Vector3 localOrWorldPos)
            => FactoryHelpers.SpawnInternal(worker.ModelPrefab, parent, localOrWorldPos);

        public GameObject Spawn(GameObject prefab, Transform parent, Vector3 localOrWorldPos)
            => FactoryHelpers.SpawnInternal(prefab, parent, localOrWorldPos);

        public void Despawn(WorkerDefinition worker, GameObject instance)
            => FactoryHelpers.DespawnInternal(worker.ModelPrefab, instance);

        public void Despawn(GameObject prefab, GameObject instance)
            => FactoryHelpers.DespawnInternal(prefab, instance);
    }
}

