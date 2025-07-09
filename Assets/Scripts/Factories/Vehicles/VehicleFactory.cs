using UnityEngine;
using Harvey.Farm.Utilities;

namespace Harvey.Farm.Factory
{
    public class VehicleFactory : Singleton<VehicleFactory>, IVehicleFactory
    {
        PoolManager pool;

        protected override void Awake()
        {
            base.Awake();
            pool = PoolManager.Instance;
        }

        /* -------- IVehicleFactory -------- */
        public GameObject Spawn(VehicleDefinition vehicle, Transform parent, Vector3 localOrWorldPos)
        {
            if (pool == null) pool = PoolManager.Instance;

            var go = pool.GetOrInstantiate(vehicle.ModelPrefab, parent);

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

        public void Despawn(VehicleDefinition vehicle, GameObject instance)
            => pool.Release(vehicle.ModelPrefab, instance);
    }
}
