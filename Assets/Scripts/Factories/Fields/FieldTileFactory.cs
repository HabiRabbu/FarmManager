using Harvey.Data.Coffee;
using UnityEngine;

namespace Harvey.Farm.Factory
{
    public class FieldTileFactory : Singleton<FieldTileFactory>, IFieldTileFactory
    {
        [SerializeField] PrefabRegistry fieldRegistry;

        /* -------- IFieldTileFactory -------- */
        public GameObject Spawn(string fieldTileGuid, Transform parent, Vector3 pos)
        {
            var prefab = fieldRegistry.Get(fieldTileGuid);
            if (!prefab)
            {
                Debug.LogError($"FieldTileFactory: prefab GUID '{fieldTileGuid}' not found");
                return null;
            }
            return FactoryHelpers.SpawnInternal(prefab, parent, pos);
        }

        public void Despawn(string fieldTileGuid, GameObject inst)
        {
            var prefab = fieldRegistry.Get(fieldTileGuid);
            FactoryHelpers.DespawnInternal(prefab, inst);
        }

        public GameObject Spawn(GameObject prefab, Transform parent, Vector3 pos) =>
            FactoryHelpers.SpawnInternal(prefab, parent, pos);
        public void Despawn(GameObject prefab, GameObject inst) =>
            FactoryHelpers.DespawnInternal(prefab, inst);
    }
}
