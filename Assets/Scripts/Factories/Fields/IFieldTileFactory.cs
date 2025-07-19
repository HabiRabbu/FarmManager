using Harvey.Data.Coffee;
using UnityEngine;

namespace Harvey.Farm.Factory
{
    public interface IFieldTileFactory
    {
        GameObject Spawn(string prefabGuid, Transform parent, Vector3 worldPos);
        void Despawn(string prefabGuid, GameObject instance);
    }
}
