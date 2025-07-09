using System;
using Harvey.Farm.Buildings;
using UnityEngine;

[DisallowMultipleComponent]
public class VehicleStats : MonoBehaviour
{
    [SerializeField] VehicleDefinition definition;
    public VehicleDefinition Def => definition;

    string id = string.Empty;
    public string Id => id;

    private GarageBuilding Home;

    public GarageBuilding GetHome() => Home;
    public void SetHome(GarageBuilding house) => Home = house;

    private bool isInitialised = false;


    /* ---------- Tunable stats ---------- */
    [field: SerializeField] public string DisplayName { get; private set; } = "Vehicle";
    [field: SerializeField] public Sprite Icon { get; private set; } = null;
    [field: SerializeField] public float MoveSpeed { get; private set; } = 2f;
    [field: SerializeField] public float Fuel { get; private set; } = 100f;
    [field: SerializeField] public float Durability { get; private set; } = 100f;
    [field: SerializeField] public float Price { get; private set; } = 100f;
    [field: SerializeField] public float Capacity { get; private set; } = 100f;

    public bool IsBusy { get; private set; }

    void Awake()
    {
        if (string.IsNullOrEmpty(id))
            id = Guid.NewGuid().ToString("N");

        if (definition == null)
        {
            return;
        }
        DisplayName = definition.DisplayName;
        MoveSpeed = definition.MoveSpeed;
        Capacity = definition.Capacity;
        Price = definition.Price;

        isInitialised = true;
    }

    public void Init(VehicleDefinition def, GarageBuilding origin)
    {
        if (isInitialised)
        {
            Debug.LogWarning("VehicleStats is already initialised. Reinitialisation may cause issues.");
            return;
        }

        definition = def ?? throw new ArgumentNullException(nameof(def), "VehicleDefinition cannot be null.");
        Awake();

        Home = origin;
    }

    public void SetBusy(bool value) => IsBusy = value;
}
