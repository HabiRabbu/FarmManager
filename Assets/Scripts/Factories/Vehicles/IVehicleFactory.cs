using Harvey.Farm.Workers;
using UnityEngine;

namespace Harvey.Farm.Factory
{
    public interface IVehicleFactory
    {
        GameObject Spawn(string guid, Transform parent, Vector3 worldPos);
        void Despawn(string guid, GameObject instance);
    }
}