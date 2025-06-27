using UnityEngine;

namespace Harvey.Farm.Buildings
{
    public abstract class Building : MonoBehaviour
    {
        [field: SerializeField] public string DisplayName { get; private set; }

        protected virtual void Awake()
        {
            BuildingManager.Instance.Register(this);
        }
        protected virtual void OnDestroy()
        {
            BuildingManager.Instance.Unregister(this);
        }
    }
}
