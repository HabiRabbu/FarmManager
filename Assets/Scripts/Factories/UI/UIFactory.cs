using UnityEngine;

namespace Harvey.Farm.Factory
{
    public class UIFactory : Singleton<UIFactory>, IUIFactory
    {
        /* ------------ IUIFactory ------------ */
        public GameObject Spawn(GameObject prefab, Transform parent)
            => AddressableFactoryHelpers.SpawnUIInternal(prefab, parent);

        public void Despawn(GameObject prefab, GameObject instance)
            => AddressableFactoryHelpers.DespawnUIInternal(prefab, instance);
    }
}
