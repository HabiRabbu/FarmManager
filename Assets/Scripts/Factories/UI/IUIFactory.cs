using UnityEngine;

namespace Harvey.Farm.Factory
{
    public interface IUIFactory
    {
        GameObject Spawn(GameObject prefab, Transform parent);
        void       Despawn(GameObject prefab, GameObject instance);
    }
}
