using UnityEngine;
using System.Threading.Tasks;

namespace Harvey.Farm.Factory
{
    public class ImplementFactory : Singleton<ImplementFactory>, IImplementFactory
    {
        /* -------- IImplementFactory -------- */
        public GameObject Spawn(string guid, Transform parent, Vector3 localOrWorldPos)
        {
            return AddressableFactoryHelpers.SpawnInternal(guid, parent, localOrWorldPos);
        }

        public async Task<GameObject> SpawnAsync(string guid, Transform parent, Vector3 localOrWorldPos)
        {
            return await AddressableFactoryHelpers.SpawnInternalAsync(guid, parent, localOrWorldPos);
        }

        public void Despawn(string guid, GameObject instance)
        {
            AddressableFactoryHelpers.DespawnInternal(guid, instance);
        }
    }
}
