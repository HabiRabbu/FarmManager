using Harvey.Data.Coffee;
using UnityEngine;

namespace Harvey.Farm.Factory
{
    public interface IFieldTileFactory
    {
        GameObject Spawn(GameObject prefab, Transform parent, Vector3 worldPos);
        void Despawn(GameObject prefab, GameObject instance);

        GameObject Spawn(string prefabGuid, Transform parent, Vector3 worldPos);
        void Despawn(string prefabGuid, GameObject instance);
    }
}
