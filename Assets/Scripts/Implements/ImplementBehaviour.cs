using UnityEngine;
using Harvey.Farm.Buildings;
using System;

namespace Harvey.Farm.Implements
{
    public class ImplementBehaviour : MonoBehaviour
    {
        [SerializeField] public string id { get; private set; } = string.Empty;

        [SerializeField] float currentDurability;

        public ImplementDefinition Def { get; private set; }
        public ShedBuilding home;
        public JobType Job; 
        
        //Definition Getters
        public float Durability => currentDurability;
        public string DisplayName => Def != null ? Def.DisplayName : string.Empty;
        public Sprite Icon => Def != null ? Def.Icon : null;

        void Awake()
        {
            if (string.IsNullOrEmpty(id))
                id = Guid.NewGuid().ToString("N");
        }

        public void Init(ImplementDefinition def, ShedBuilding origin)
        {
            if (string.IsNullOrEmpty(id))
                id = Guid.NewGuid().ToString("N");

            Def = def;
            home = origin;
            Job = def.Job;

            currentDurability = 1f;
        }

        // *------------------------ PUBLIC API ------------------------*
        public void ApplyWear(float amount) =>
            currentDurability = Mathf.Max(0, currentDurability - amount);

        /* attach to tractor */
        public void AttachTo(Transform hitch)
        {
            transform.SetParent(hitch, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        /* detach & send back */
        public void Detach()
        {
            transform.SetParent(null, true);
        }
    }
}
