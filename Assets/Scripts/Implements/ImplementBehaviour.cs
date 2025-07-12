using UnityEngine;
using Harvey.Farm.Buildings;
using System;

namespace Harvey.Farm.Implements
{
    public class ImplementBehaviour : MonoBehaviour
    {
        [SerializeField] float currentDurability;

        public ImplementDefinition Def { get; private set; }
        public ShedBuilding home;
        public GuidBehaviour guidBehaviour;
        public JobType Job;

        //Getters/Setters
        public float Durability => currentDurability;
        public string DisplayName => Def != null ? Def.DisplayName : string.Empty;
        public Sprite Icon => Def != null ? Def.Icon : null;
        public string GetId() => guidBehaviour.GetId();
        public void SetId(string newId) => guidBehaviour.SetId(newId);

        void Awake()
        {
            guidBehaviour = GetComponent<GuidBehaviour>();
        }

        public void Init(ImplementDefinition def, ShedBuilding origin)
        {

            Def = def;
            home = origin;
            Job = def.Job;

            currentDurability = 1f;
        }

        // *------------------------ PUBLIC API ------------------------*
        public void ApplyWear(float amount) =>
            currentDurability = Mathf.Max(0, currentDurability - amount);

        public void AttachTo(Transform hitch)
        {
            transform.SetParent(hitch, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        public void Detach()
        {
            transform.SetParent(null, true);
        }
    }
}
