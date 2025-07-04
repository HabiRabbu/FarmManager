using UnityEngine;

namespace Harvey.Farm.Buildings
{
    public abstract class Building : MonoBehaviour
    {
        /// <summary>The data object that drives this building.</summary>
        public abstract BuildingDefinition Definition { get; }
        public string DisplayName => Definition?.DisplayName ?? name;

        #region Auto-registration
        protected virtual void Start() => BuildingManager.Instance.Register(this);
        protected virtual void OnDestroy() => BuildingManager.Instance.Unregister(this);
        #endregion
    }
}
