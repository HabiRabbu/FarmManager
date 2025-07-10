using Harvey.Data.Coffee;
using UnityEngine;

namespace Harvey.Farm.Factory
{
    public interface ICropFactory
    {
        GameObject Spawn(GameObject prefab, Transform parent, Vector3 worldPos);
        GameObject Spawn(CoffeeCropData crop, Transform parent, Vector3 worldPos);
        void Despawn(GameObject prefab, GameObject instance);
        void Despawn(CoffeeCropData crop, GameObject instance);
    }
}
