using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic GameObject pooling.  
/// Call Get(prefab,parent) to reuse or Instantiate.  
/// Call Release(prefab, instance) to deactivate and enqueue.
/// </summary>
namespace Harvey.Farm.Utilities
{
    public class PoolManager : Singleton<PoolManager>
    {

        private readonly Dictionary<GameObject, Queue<GameObject>> pools = new();

        public GameObject GetOrInstantiate(GameObject prefab, Transform parent = null)
        {
            if (!pools.TryGetValue(prefab, out var q))
            {
                q = new Queue<GameObject>();
                pools[prefab] = q;
            }

            GameObject go = (q.Count > 0)
                ? q.Dequeue()
                : Instantiate(prefab);

            go.transform.SetParent(parent, false);
            go.SetActive(true);
            return go;
        }

        public void Release(GameObject prefab, GameObject instance)
        {
            instance.SetActive(false);
            pools[prefab].Enqueue(instance);
        }
    }
}
