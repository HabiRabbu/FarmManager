using UnityEngine;
using Harvey.Farm.Utilities;
using System.Threading.Tasks;

namespace Harvey.Farm.Factory
{
    /// <summary>
    /// Unified factory helpers using Addressables + Pooling.
    /// All spawning goes through AddressablePoolManager for consistency.
    /// </summary>
    public static class AddressableFactoryHelpers
    {
        /* ------------ Addressable GameObject Spawning ------------ */
        
        /// <summary>
        /// Spawn a GameObject by Addressable GUID (async - recommended).
        /// </summary>
        public static async Task<GameObject> SpawnInternalAsync(
            string guid,
            Transform parent,
            Vector3 localOrWorldPos)
        {
            var pool = AddressablePoolManager.Instance;
            var go = await pool.GetOrInstantiateAsync(guid, parent);

            if (go == null) return null;

            ConfigureSpawnedObject(go, parent, localOrWorldPos);
            return go;
        }

        /// <summary>
        /// Spawn a GameObject by Addressable GUID (synchronous - only works if prefab is already cached).
        /// If the prefab isn't cached, this will return null and log a warning.
        /// Use SpawnInternalAsync() for guaranteed loading.
        /// </summary>
        public static GameObject SpawnInternal(
            string guid,
            Transform parent,
            Vector3 localOrWorldPos)
        {
            var pool = AddressablePoolManager.Instance;
            var cachedPrefab = pool.GetCachedPrefab(guid);
            
            if (cachedPrefab == null)
            {
                Debug.LogWarning($"AddressableFactoryHelpers: Cannot spawn '{guid}' synchronously - prefab not cached. Use SpawnInternalAsync() or preload the asset.");
                return null;
            }

            var go = pool.GetOrInstantiateFromPrefab(cachedPrefab, parent);
            ConfigureSpawnedObject(go, parent, localOrWorldPos);
            return go;
        }

        /// <summary>
        /// Despawn a GameObject spawned by GUID.
        /// </summary>
        public static void DespawnInternal(string guid, GameObject instance)
        {
            var pool = AddressablePoolManager.Instance;
            pool.Release(guid, instance);
        }

        /* ------------ UI GameObject Spawning ------------ */
        
        /// <summary>
        /// Spawn a UI GameObject by Addressable GUID (async).
        /// </summary>
        public static async Task<GameObject> SpawnUIInternalAsync(
            string guid,
            Transform parent)
        {
            var pool = AddressablePoolManager.Instance;
            var go = await pool.GetOrInstantiateAsync(guid, parent);

            if (go == null) return null;

            ConfigureUIObject(go);
            return go;
        }

        /// <summary>
        /// Spawn a UI GameObject by Addressable GUID (synchronous - only works if prefab is cached).
        /// </summary>
        public static GameObject SpawnUIInternal(string guid, Transform parent)
        {
            var pool = AddressablePoolManager.Instance;
            var cachedPrefab = pool.GetCachedPrefab(guid);
            
            if (cachedPrefab == null)
            {
                Debug.LogWarning($"AddressableFactoryHelpers: Cannot spawn UI '{guid}' synchronously - prefab not cached. Use SpawnUIInternalAsync() or preload the asset.");
                return null;
            }

            var go = pool.GetOrInstantiateFromPrefab(cachedPrefab, parent);
            ConfigureUIObject(go);
            return go;
        }

        /// <summary>
        /// Spawn a UI GameObject from direct prefab reference.
        /// </summary>
        public static GameObject SpawnUIInternal(GameObject prefab, Transform parent)
        {
            var pool = AddressablePoolManager.Instance;
            var go = pool.GetOrInstantiateFromPrefab(prefab, parent);

            ConfigureUIObject(go);
            return go;
        }

        /// <summary>
        /// Despawn a UI GameObject spawned by GUID.
        /// </summary>
        public static void DespawnUIInternal(string guid, GameObject instance)
            => AddressablePoolManager.Instance.Release(guid, instance);

        /// <summary>
        /// Despawn a UI GameObject spawned from direct prefab.
        /// </summary>
        public static void DespawnUIInternal(GameObject prefab, GameObject instance)
            => AddressablePoolManager.Instance.Release(prefab, instance);

        /* ------------ Private Helpers ------------ */

        private static void ConfigureSpawnedObject(GameObject go, Transform parent, Vector3 localOrWorldPos)
        {
            var guidBehaviour = go.GetComponent<GuidBehaviour>();
            if (guidBehaviour != null)
            {
                guidBehaviour.GenerateNewGuid();
            }

            if (parent)
            {
                go.transform.SetLocalPositionAndRotation(localOrWorldPos, Quaternion.identity);
            }
            else
            {
                go.transform.SetPositionAndRotation(localOrWorldPos, Quaternion.identity);
            }

            go.transform.localScale = Vector3.one;
        }

        private static void ConfigureUIObject(GameObject go)
        {
            var rt = go.GetComponent<RectTransform>();
            if (rt == null) rt = go.AddComponent<RectTransform>();

            rt.localScale = Vector3.one;
            rt.localRotation = Quaternion.identity;
        }
    }
}
