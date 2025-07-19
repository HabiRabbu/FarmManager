using Harvey.Data.Coffee;
using UnityEngine;
using System.Threading.Tasks;

namespace Harvey.Farm.Factory
{
    public class FieldTileFactory : Singleton<FieldTileFactory>, IFieldTileFactory
    {
        /* -------- IFieldTileFactory -------- */
        public GameObject Spawn(string fieldTileGuid, Transform parent, Vector3 pos)
        {
            return AddressableFactoryHelpers.SpawnInternal(fieldTileGuid, parent, pos);
        }

        public async Task<GameObject> SpawnAsync(string fieldTileGuid, Transform parent, Vector3 pos)
        {
            return await AddressableFactoryHelpers.SpawnInternalAsync(fieldTileGuid, parent, pos);
        }

        public void Despawn(string fieldTileGuid, GameObject inst)
        {
            AddressableFactoryHelpers.DespawnInternal(fieldTileGuid, inst);
        }
    }
}
