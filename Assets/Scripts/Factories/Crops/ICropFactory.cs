using UnityEngine;
using Harvey.Farm.Crops;

namespace Harvey.Farm.Factory
{
    public interface ICropFactory
    {
        GameObject Spawn(CropDefinition crop, Transform parent, Vector3 worldPos);
        void       Despawn(CropDefinition crop, GameObject instance);
    }
}
