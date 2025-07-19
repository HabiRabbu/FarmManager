using System;
using Harvey.Farm.Buildings;
using Harvey.Farm.Fields;
using Harvey.Farm.VehicleScripts;
using UnityEngine;

[DisallowMultipleComponent]
public class VehicleStats : MonoBehaviour
{
    [SerializeField] VehicleDefinition definition;
    public VehicleDefinition Def => definition;

    Vehicle vehicleController;
    public Vehicle VehicleController => vehicleController;

    // Field Stuff
    public FieldController CurrentField;
    public int CurrentTileIndex { get; private set; } = 0;
    public void SetCurrentTileIndex(int index) => CurrentTileIndex = index;

    // Garage Building
    private GarageBuilding Home;
    public GarageBuilding GetHome() => Home;
    public void SetHome(GarageBuilding house) => Home = house;

    // GuidBehaviour
    GuidBehaviour guidBehaviour;
    public string GetId() => guidBehaviour.GetId();
    public void SetId(string newId) => guidBehaviour.SetId(newId);

    /* ---------- Model ---------- */
    public VehicleModel Model;
    public void SetModel(VehicleModel model) => Model = model;

    public bool IsBusy => Model.IsBusy;
    public void SetBusy(bool value) => Model.IsBusy = value;


    void Awake()
    {
        guidBehaviour = GetComponent<GuidBehaviour>();
        vehicleController = GetComponent<Vehicle>();
    }

    public void InitFromModel(VehicleModel model, GarageBuilding home)
    {
        if (model == null) return;
        Debug.Log($"VehicleStats: Initializing from model {model.DisplayName} with Home: {home}");

        Model = model;
        Home = home;
        guidBehaviour.SetId(model.Id);
        gameObject.name = model.DisplayName;
        VehicleManager.Instance.RegisterVehicle(vehicleController);
    }

    void Start()
    {
        if (definition != null)
        {
            Debug.Log($"VehicleStats: Initializing from definition {definition.DisplayName} with Home: {Home}");
            switch (definition.Type)
            {
                case VehicleType.Tractor:
                    Model = VehicleMapper.FromDefinition<TractorModel>(definition, Home, guidBehaviour.GetId());
                    break;
                case VehicleType.CombineHarvester:
                    Model = VehicleMapper.FromDefinition<HarvesterModel>(definition, Home, guidBehaviour.GetId());
                    break;
                default:
                    throw new System.ArgumentException($"Unsupported vehicle type: {definition.Type}");
            }

            Home = BuildingManager.Instance.GetById<GarageBuilding>(Model.HomeId);
            gameObject.name = Model.DisplayName;
            VehicleManager.Instance.RegisterVehicle(vehicleController);
        }
    }
}
