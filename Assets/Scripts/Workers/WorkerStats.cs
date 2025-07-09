using System;
using Harvey.Farm.Buildings;
using UnityEngine;

namespace Harvey.Farm.Workers
{
    [DisallowMultipleComponent]
    public class WorkerStats : MonoBehaviour
    {
        [SerializeField] WorkerDefinition definition;
        public WorkerDefinition Def => definition;
        string id = string.Empty;
        public string Id => id;

        private HouseBuilding Home;
        public HouseBuilding GetHome() => Home;
        public void SetHome(HouseBuilding house) => Home = house;

        /* ---------- Tunable stats ---------- */
        public string WorkerName { get; private set; }
        public float WalkSpeed { get; private set; }
        public bool IsBusy { get; private set; }

        void Awake()
        {
            if (string.IsNullOrEmpty(id))
                id = Guid.NewGuid().ToString("N");
        }

        public void SetBusy(bool value) => IsBusy = value;

        public void InjectDefinition(WorkerDefinition def)
        {
            this.definition = def;
            WorkerName = def.DisplayName;
            //WalkSpeed = def.WalkSpeed;
            WalkSpeed = UnityEngine.Random.Range(1f, 3f); // Temporary random speed for testing TODO: Change this to use the definition's speed
        }

        public float GetActionDuration(JobType type)
        {
            return type switch
            {
                JobType.Plow => Def.plowSeconds,
                JobType.Seed => Def.seedSeconds,
                JobType.Harvest => Def.harvestSeconds,
                _ => 0f
            };


            //TODO: Later can scale it based on skill and fatigue and stuff 
            // return base * (1f / skillLevel) or * fatigueCurve;
        }

    }
}