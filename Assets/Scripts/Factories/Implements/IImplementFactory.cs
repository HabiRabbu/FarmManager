using UnityEngine;

namespace Harvey.Farm.Factory
{
    public interface IImplementFactory
    {
        GameObject Spawn(ImplementDefinition tool, Transform parent, Vector3 worldPos);
        void       Despawn(ImplementDefinition tool, GameObject instance);
    }
}
