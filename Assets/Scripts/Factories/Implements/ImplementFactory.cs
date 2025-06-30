using UnityEngine;
using Harvey.Farm.Utilities;  // PoolManager

namespace Harvey.Farm.Factory
{
    /// <summary>
    /// Central place to fetch / release crop visuals.
    /// Wraps PoolManager so the rest of the game never talks to Instantiate/Destroy.
    /// </summary>
    public class ImplementFactory : Singleton<ImplementFactory>, IImplementFactory
    {
        PoolManager pool;

        protected override void Awake()
        {
            base.Awake();
            pool = PoolManager.Instance;
        }

        /* -------- ICropFactory -------- */
        public GameObject Spawn(ImplementDefinition tool, Transform parent, Vector3 localOrWorldPos)
        {
            if (pool == null) pool = PoolManager.Instance;

            var go = pool.GetOrInstantiate(tool.ModelPrefab, parent);

            if (parent != null)
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

        public void Despawn(ImplementDefinition tool, GameObject instance)
            => pool.Release(tool.ModelPrefab, instance);
    }
}
