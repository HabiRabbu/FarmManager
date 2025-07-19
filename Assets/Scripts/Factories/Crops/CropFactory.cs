using Harvey.Data.Coffee;
using UnityEngine;
using System.Threading.Tasks;

namespace Harvey.Farm.Factory
{
    /// <summary>
    /// Central place to fetch / release crop visuals.
    /// Uses Addressables for GUID-based spawning with pooling.
    /// </summary>
    public class CropFactory : Singleton<CropFactory>, ICropFactory
    {
        /* -------- ICropFactory -------- */
        public GameObject Spawn(CoffeeCropData crop, Transform parent, Vector3 pos)
        {
            return AddressableFactoryHelpers.SpawnInternal(crop.PrefabGuid, parent, pos);
        }

        public async Task<GameObject> SpawnAsync(CoffeeCropData crop, Transform parent, Vector3 pos)
        {
            return await AddressableFactoryHelpers.SpawnInternalAsync(crop.PrefabGuid, parent, pos);
        }

        public void Despawn(CoffeeCropData crop, GameObject inst)
        {
            AddressableFactoryHelpers.DespawnInternal(crop.PrefabGuid, inst);
        }
    }
}
