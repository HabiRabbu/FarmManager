using UnityEngine;
using Harvey.Farm.Utilities;
using Harvey.Farm.Workers;  // PoolManager

namespace Harvey.Farm.Factory
{
    public class WorkerFactory : Singleton<WorkerFactory>, IWorkerFactory
    {
        PoolManager pool;

        protected override void Awake()
        {
            base.Awake();
            pool = PoolManager.Instance;
        }

        /* -------- IWorkerFactory -------- */
        public GameObject Spawn(WorkerDefinition worker, Transform parent, Vector3 localOrWorldPos)
        {
            if (pool == null) pool = PoolManager.Instance;

            var go = pool.GetOrInstantiate(worker.ModelPrefab, parent);

            if (parent != null)
            {
                go.transform.SetLocalPositionAndRotation(localOrWorldPos, Quaternion.identity);
            }
            else
            {
                go.transform.SetPositionAndRotation(localOrWorldPos, Quaternion.identity);
            }

            go.transform.localScale = Vector3.one;
            return go;
        }

        public void Despawn(WorkerDefinition worker, GameObject instance)
            => pool.Release(worker.ModelPrefab, instance);
    }
}
