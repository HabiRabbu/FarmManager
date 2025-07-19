using Harvey.Data.Coffee;
using Harvey.Data.Fields;
using UnityEngine;
using System.Threading.Tasks;

namespace Harvey.Farm.Factory
{
    public class FieldFactory : Singleton<FieldFactory>, IFieldFactory
    {
        /* -------- IFieldFactory -------- */
        public GameObject Spawn(string fieldGuid, Transform parent, Vector3 pos)
        {
            return AddressableFactoryHelpers.SpawnInternal(fieldGuid, parent, pos);
        }

        public async Task<GameObject> SpawnAsync(string fieldGuid, Transform parent, Vector3 pos)
        {
            return await AddressableFactoryHelpers.SpawnInternalAsync(fieldGuid, parent, pos);
        }

        public void Despawn(string fieldGuid, GameObject inst)
        {
            AddressableFactoryHelpers.DespawnInternal(fieldGuid, inst);
        }
    }
}
