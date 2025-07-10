using UnityEngine;
using Harvey.Farm.Utilities;

namespace Harvey.Farm.Factory
{
    public static partial class FactoryHelpers
    {
        /* ------------ GameObject (Transform) ------------ */
        public static GameObject SpawnInternal(
            GameObject prefab,
            Transform parent,
            Vector3 localOrWorldPos)
        {
            var pool = PoolManager.Instance;

            var go = pool.GetOrInstantiate(prefab, parent);

            if (parent)
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

        public static void DespawnInternal(
            GameObject prefab,
            GameObject instance)
        {
            var pool = PoolManager.Instance;
            pool.Release(prefab, instance);
        }
    }

    public static partial class FactoryHelpers
    {
        /* ------------ UI  (RectTransform) ------------ */
        public static GameObject SpawnUIInternal(
            GameObject prefab,
            Transform parent)
        {
            var pool = PoolManager.Instance;
            var go = pool.GetOrInstantiate(prefab, parent);

            var rt = go.GetComponent<RectTransform>();
            if (rt == null) rt = go.AddComponent<RectTransform>();

            rt.localScale = Vector3.one;
            rt.localRotation = Quaternion.identity;

            return go;
        }

        public static void DespawnUIInternal(GameObject prefab, GameObject instance)
            => PoolManager.Instance.Release(prefab, instance);
    }
}

