using UnityEngine;

namespace Harvey.Farm.Buildings
{
    public abstract class Building : MonoBehaviour
    {

        public BuildingModel Model { get; private set; }
        public void SetModel(BuildingModel model)
        {
            Model = model;
        }

        // GuidBehaviour
        GuidBehaviour guidBehaviour;
        public string GetId() => guidBehaviour.GetId();
        public void SetId(string newId) => guidBehaviour.SetId(newId);

        public virtual void Awake()
        {
            guidBehaviour = GetComponent<GuidBehaviour>();
        }

        /// <summary>The data object that drives this building.</summary>
        public abstract BuildingDefinition Definition { get; }
        public string DisplayName => Definition?.DisplayName ?? name;

        protected virtual void OnDestroy() => BuildingManager.Instance.Unregister(this);
    }
}
