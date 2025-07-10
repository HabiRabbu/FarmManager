using Harvey.Farm.Workers;
using UnityEngine;

namespace Harvey.Farm.Factory
{
    public interface IVehicleFactory
    {
        GameObject Spawn(VehicleDefinition vehicle, Transform parent, Vector3 worldPos);
        GameObject Spawn(GameObject prefab, Transform parent, Vector3 worldPos);
        void Despawn(VehicleDefinition vehicle, GameObject instance);
        void Despawn(GameObject prefab, GameObject instance);
    }
}