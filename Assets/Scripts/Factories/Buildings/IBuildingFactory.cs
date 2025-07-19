using Harvey.Data.Coffee;
using Harvey.Data.Fields;
using UnityEngine;
using System.Threading.Tasks;

namespace Harvey.Farm.Factory
{
    public interface IBuildingFactory
    {
        GameObject Spawn(string guid, Transform parent, Vector3 worldPos);
        Task<GameObject> SpawnAsync(string guid, Transform parent, Vector3 worldPos);
        void Despawn(string guid, GameObject instance);
    }
}
