using Harvey.Data.Coffee;
using Harvey.Data.Fields;
using UnityEngine;

namespace Harvey.Farm.Factory
{
    public interface IFieldFactory
    {
        GameObject Spawn(GameObject prefab, Transform parent, Vector3 worldPos);
        GameObject Spawn(FieldSaveData field, Transform parent, Vector3 worldPos);
        void Despawn(GameObject prefab, GameObject instance);
        void Despawn(FieldSaveData field, GameObject instance);
    }
}
