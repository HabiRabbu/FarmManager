using Harvey.Farm.Workers;
using UnityEngine;

namespace Harvey.Farm.Factory
{
    public interface IWorkerFactory
    {
        GameObject Spawn(WorkerDefinition worker, Transform parent, Vector3 worldPos);
        void       Despawn(WorkerDefinition worker, GameObject instance);
    }
}