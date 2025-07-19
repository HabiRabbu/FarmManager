using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;

namespace Harvey.Farm.Utilities
{
    public class AddressablePoolManager : Singleton<AddressablePoolManager>
    {
        private readonly Dictionary<string, GameObject> loadedPrefabs = new();
        
        private readonly Dictionary<GameObject, Queue<GameObject>> pools = new();
        
        private readonly Dictionary<string, AsyncOperationHandle<GameObject>> loadingOps = new();

        public async Task<GameObject> GetOrInstantiateAsync(string guid, Transform parent = null)
        {
            var prefab = await LoadPrefabAsync(guid);
            if (prefab == null)
            {
                Debug.LogError($"AddressablePoolManager: Failed to load prefab with GUID '{guid}'");
                return null;
            }

            return GetOrInstantiateFromPrefab(prefab, parent);
        }

        public GameObject GetOrInstantiateFromPrefab(GameObject prefab, Transform parent = null)
        {
            if (!pools.TryGetValue(prefab, out var queue))
            {
                queue = new Queue<GameObject>();
                pools[prefab] = queue;
            }

            GameObject instance = (queue.Count > 0)
                ? queue.Dequeue()
                : Instantiate(prefab);

            instance.transform.SetParent(parent, false);
            instance.SetActive(true);
            return instance;
        }

        public void Release(GameObject prefab, GameObject instance)
        {
            if (!pools.TryGetValue(prefab, out var queue))
            {
                queue = new Queue<GameObject>();
                pools[prefab] = queue;
            }

            instance.SetActive(false);
            queue.Enqueue(instance);
        }

        public void Release(string guid, GameObject instance)
        {
            if (loadedPrefabs.TryGetValue(guid, out var prefab))
            {
                Release(prefab, instance);
            }
            else
            {
                Debug.LogWarning($"AddressablePoolManager: Cannot release instance - prefab with GUID '{guid}' not found in cache");
            }
        }

        public async Task<GameObject> LoadPrefabAsync(string guid)
        {
            if (loadedPrefabs.TryGetValue(guid, out var cachedPrefab))
            {
                return cachedPrefab;
            }

            if (loadingOps.TryGetValue(guid, out var existingOp))
            {
                await existingOp.Task;
                return existingOp.Result;
            }

            var handle = Addressables.LoadAssetAsync<GameObject>(guid);
            loadingOps[guid] = handle;

            try
            {
                var prefab = await handle.Task;
                if (prefab != null)
                {
                    loadedPrefabs[guid] = prefab;
                    Debug.Log($"AddressablePoolManager: Loaded prefab '{prefab.name}' with GUID '{guid}'");
                }
                return prefab;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"AddressablePoolManager: Failed to load GUID '{guid}': {e.Message}");
                return null;
            }
            finally
            {
                loadingOps.Remove(guid);
            }
        }

        public GameObject GetCachedPrefab(string guid)
        {
            return loadedPrefabs.TryGetValue(guid, out var prefab) ? prefab : null;
        }

        public async Task PreloadAsync(string guid)
        {
            await LoadPrefabAsync(guid);
        }

        void OnDestroy()
        {
            foreach (var handle in loadingOps.Values)
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
            }

            foreach (var guid in loadedPrefabs.Keys)
            {
                var prefab = loadedPrefabs[guid];
                if (prefab != null)
                {
                    Addressables.Release(prefab);
                }
            }

            loadedPrefabs.Clear();
            loadingOps.Clear();
            pools.Clear();
        }
    }
}
