using UnityEngine;
using Harvey.Farm.Utilities;  // PoolManager

namespace Harvey.Farm.Factory
{
    public class ImplementFactory : Singleton<ImplementFactory>, IImplementFactory
    {
        /* -------- IImplementFactory -------- */
        public GameObject Spawn(ImplementDefinition def, Transform parent, Vector3 localOrWorldPos)
            => FactoryHelpers.SpawnInternal(def.ModelPrefab, parent, localOrWorldPos);

        public GameObject Spawn(GameObject prefab, Transform parent, Vector3 localOrWorldPos)
            => FactoryHelpers.SpawnInternal(prefab, parent, localOrWorldPos);

        public void Despawn(ImplementDefinition def, GameObject instance)
            => FactoryHelpers.DespawnInternal(def.ModelPrefab, instance);

        public void Despawn(GameObject prefab, GameObject instance)
            => FactoryHelpers.DespawnInternal(prefab, instance);
    }
}
