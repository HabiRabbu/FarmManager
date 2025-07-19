using Harvey.Data.Coffee;
using Harvey.Data.Fields;
using UnityEngine;

namespace Harvey.Farm.Factory
{
    public interface IFieldFactory
    {
        GameObject Spawn(string fieldGuid, Transform parent, Vector3 worldPos);
        void Despawn(string fieldGuid, GameObject instance);
    }
}
