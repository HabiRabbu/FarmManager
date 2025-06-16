using UnityEngine;

/// <summary>
/// Wraps PoolManager for UI so you only need prefab + parent once. (Z-index issues)
/// </summary>
namespace Harvey.Farm.Utilities
{
    public class UIPrefabPool
    {
        private readonly GameObject prefab;
        private readonly Transform parent;

        public UIPrefabPool(GameObject prefab, Transform parent)
        {
            this.prefab = prefab;
            this.parent = parent;
        }

        public GameObject UIGetOrInstantiate()
        {
            return PoolManager.Instance.GetOrInstantiate(prefab, parent);
        }

        public void UIRelease(GameObject instance)
        {
            PoolManager.Instance.Release(prefab, instance);
        }
    }
}

