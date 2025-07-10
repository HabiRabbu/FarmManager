using UnityEngine;
using Harvey.Farm.Utilities;

namespace Harvey.Farm.Factory
{
    public class VehicleFactory : Singleton<VehicleFactory>, IVehicleFactory
    {
        /* -------- IVehicleFactory -------- */
        public GameObject Spawn(VehicleDefinition def, Transform parent, Vector3 localOrWorldPos)
            => FactoryHelpers.SpawnInternal(def.ModelPrefab, parent, localOrWorldPos);

        public GameObject Spawn(GameObject prefab, Transform parent, Vector3 localOrWorldPos)
            => FactoryHelpers.SpawnInternal(prefab, parent, localOrWorldPos);

        public void Despawn(VehicleDefinition def, GameObject instance)
            => FactoryHelpers.DespawnInternal(def.ModelPrefab, instance);

        public void Despawn(GameObject prefab, GameObject instance)
            => FactoryHelpers.DespawnInternal(prefab, instance);
    }
}
