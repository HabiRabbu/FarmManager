using Harvey.Data.Coffee;
using Harvey.Data.Fields;
using UnityEngine;

namespace Harvey.Farm.Factory
{
    public class FieldFactory : Singleton<FieldFactory>, IFieldFactory
    {
        [SerializeField] PrefabRegistry fieldRegistry;

        /* -------- IFieldFactory -------- */
        public GameObject Spawn(FieldSaveData field, Transform parent, Vector3 pos)
        {
            var prefab = fieldRegistry.Get(field.PrefabGuid);
            if (!prefab)
            {
                Debug.LogError($"FieldFactory: prefab GUID '{field.PrefabGuid}' not found");
                return null;
            }
            return FactoryHelpers.SpawnInternal(prefab, parent, pos);
        }

        public void Despawn(FieldSaveData field, GameObject inst)
        {
            var prefab = fieldRegistry.Get(field.PrefabGuid);
            FactoryHelpers.DespawnInternal(prefab, inst);
        }

        public GameObject Spawn(string fieldGuid, Transform parent, Vector3 pos)
        {
            var prefab = fieldRegistry.Get(fieldGuid);
            if (!prefab)
            {
                Debug.LogError($"FieldFactory: prefab GUID '{fieldGuid}' not found");
                return null;
            }
            return FactoryHelpers.SpawnInternal(prefab, parent, pos);
        }

        public void Despawn(string fieldGuid, GameObject inst)
        {
            var prefab = fieldRegistry.Get(fieldGuid);
            FactoryHelpers.DespawnInternal(prefab, inst);
        }

        public GameObject Spawn(GameObject prefab, Transform parent, Vector3 pos) =>
            FactoryHelpers.SpawnInternal(prefab, parent, pos);
        public void Despawn(GameObject prefab, GameObject inst) =>
            FactoryHelpers.DespawnInternal(prefab, inst);
    }
}
