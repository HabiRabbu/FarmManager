using UnityEngine;

namespace Harvey.Farm.Factory
{
    public class UIFactory : Singleton<UIFactory>, IUIFactory
    {
        /* ------------ IUIFactory ------------ */
        public GameObject Spawn(GameObject prefab, Transform parent)
            => FactoryHelpers.SpawnUIInternal(prefab, parent);

        public void Despawn(GameObject prefab, GameObject instance)
            => FactoryHelpers.DespawnUIInternal(prefab, instance);
    }
}
