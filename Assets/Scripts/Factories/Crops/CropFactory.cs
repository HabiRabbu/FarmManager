using UnityEngine;
using Harvey.Farm.Utilities;  // PoolManager
using Harvey.Farm.Crops;

namespace Harvey.Farm.Factory
{
    /// <summary>
    /// Central place to fetch / release crop visuals.
    /// Wraps PoolManager so the rest of the game never talks to Instantiate/Destroy.
    /// </summary>
    public class CropFactory : Singleton<CropFactory>, ICropFactory
    {
        PoolManager pool;

        protected override void Awake()
        {
            base.Awake();
            pool = PoolManager.Instance;
        }

        /* -------- ICropFactory -------- */
        public GameObject Spawn(CropDefinition crop, Transform parent, Vector3 localOrWorldPos)
        {
            if (pool == null) pool = PoolManager.Instance;

            var go = pool.GetOrInstantiate(crop.cropPrefab, parent);

            if (parent != null)
            {
                go.transform.SetLocalPositionAndRotation(localOrWorldPos, Quaternion.identity); // Local Space
            }
            else
            {
                go.transform.SetPositionAndRotation(localOrWorldPos, Quaternion.identity); // World Space
            }

            go.transform.localScale = Vector3.one;
            return go;
        }

        public void Despawn(CropDefinition crop, GameObject instance)
            => pool.Release(crop.cropPrefab, instance);
    }
}
