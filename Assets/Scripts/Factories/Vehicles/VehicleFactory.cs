using UnityEngine;
using System.Threading.Tasks;

namespace Harvey.Farm.Factory
{
    public class VehicleFactory : Singleton<VehicleFactory>, IVehicleFactory
    {
        /* -------- IVehicleFactory -------- */
        public GameObject Spawn(string guid, Transform parent, Vector3 pos)
        {
            return AddressableFactoryHelpers.SpawnInternal(guid, parent, pos);
        }

        public async Task<GameObject> SpawnAsync(string guid, Transform parent, Vector3 pos)
        {
            return await AddressableFactoryHelpers.SpawnInternalAsync(guid, parent, pos);
        }

        public void Despawn(string guid, GameObject inst)
        {
            AddressableFactoryHelpers.DespawnInternal(guid, inst);
        }
    }
}
