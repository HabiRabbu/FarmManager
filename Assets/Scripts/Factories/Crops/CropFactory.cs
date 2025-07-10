using Harvey.Data.Coffee;
using UnityEngine;

namespace Harvey.Farm.Factory
{
    /// <summary>
    /// Central place to fetch / release crop visuals.
    /// Wraps PoolManager so the rest of the game never talks to Instantiate/Destroy.
    /// </summary>
    public class CropFactory : Singleton<CropFactory>, ICropFactory
    {
        [SerializeField] PrefabRegistry coffeeRegistry;

        /* -------- ICropFactory -------- */
        public GameObject Spawn(CoffeeCropData crop, Transform parent, Vector3 pos)
        {
            var prefab = coffeeRegistry.Get(crop.PrefabGuid);
            if (!prefab)
            {
                Debug.LogError($"CropFactory: prefab GUID '{crop.PrefabGuid}' not found");
                return null;
            }
            return FactoryHelpers.SpawnInternal(prefab, parent, pos);
        }

        public void Despawn(CoffeeCropData crop, GameObject inst)
        {
            var prefab = coffeeRegistry.Get(crop.PrefabGuid);
            FactoryHelpers.DespawnInternal(prefab, inst);
        }

        public GameObject Spawn(GameObject prefab, Transform parent, Vector3 pos) =>
            FactoryHelpers.SpawnInternal(prefab, parent, pos);
        public void Despawn(GameObject prefab, GameObject inst) =>
            FactoryHelpers.DespawnInternal(prefab, inst);
    }
}
