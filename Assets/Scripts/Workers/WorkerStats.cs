using System;
using Harvey.Farm.Buildings;
using Harvey.Farm.Fields;
using UnityEngine;

namespace Harvey.Farm.Workers
{
    [DisallowMultipleComponent]
    public class WorkerStats : MonoBehaviour
    {
        [SerializeField] WorkerDefinition definition;
        public WorkerDefinition Def => definition;

        private Worker workerController;
        public Worker WorkerController => workerController;

        // Field Stuff
        public FieldController CurrentField;
        public int CurrentTileIndex { get; private set; } = 0;
        public void SetCurrentTileIndex(int index) => CurrentTileIndex = index;

        // House Building
        private HouseBuilding Home;
        public HouseBuilding GetHome() => Home;
        public void SetHome(HouseBuilding house) => Home = house;

        // GuidBehaviour
        GuidBehaviour guidBehaviour;
        public string GetId() => guidBehaviour.GetId();
        public void SetId(string newId) => guidBehaviour.SetId(newId);

        /* ---------- Model ---------- */
        public WorkerModel Model;
        public void SetModel(WorkerModel model) => Model = model;

        public bool IsBusy => Model.IsBusy;
        public void SetBusy(bool value) => Model.IsBusy = value;

        void Awake()
        {
            guidBehaviour = GetComponent<GuidBehaviour>();
            workerController = GetComponent<Worker>();
        }

        public void InitFromModel(WorkerModel model, HouseBuilding home)
        {
            if (model == null) return;
            Debug.Log($"WorkerStats: Initialising from model {model.DisplayName} with Home: {home}");

            Model = model;
            Home = home;
            guidBehaviour.SetId(model.Id);
            gameObject.name = model.DisplayName;
            WorkerManager.Instance.Register(workerController);
        }

        void Start()
        {
            if (definition != null)
            {
                Debug.Log($"WorkerStats: Initialising from definition {definition.DisplayName} with Home: {Home}");
                Model = WorkerMapper.FromDefinition(definition, Home, guidBehaviour.GetId());

                Home = BuildingManager.Instance.GetById<HouseBuilding>(Model.HomeId);
                gameObject.name = Model.DisplayName;
                WorkerManager.Instance.Register(workerController);
            }
        }

        public float GetActionDuration(JobType type)
        {
            return type switch
            {
                JobType.Plow => Model.plowSeconds,
                JobType.Seed => Model.seedSeconds,
                JobType.Harvest => Model.harvestSeconds,
                _ => 0f
            };


            //TODO: Later can scale it based on skill and fatigue and stuff 
            // return base * (1f / skillLevel) or * fatigueCurve;
        }

    }
}