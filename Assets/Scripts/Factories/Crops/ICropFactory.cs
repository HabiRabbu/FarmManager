using Harvey.Data.Coffee;
using UnityEngine;

namespace Harvey.Farm.Factory
{
    public interface ICropFactory
    {
        GameObject Spawn(CoffeeCropData crop, Transform parent, Vector3 worldPos);
        void Despawn(CoffeeCropData crop, GameObject instance);
    }
}
