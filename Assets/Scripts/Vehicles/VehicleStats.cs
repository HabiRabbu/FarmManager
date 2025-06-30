using UnityEngine;

[DisallowMultipleComponent]
public class VehicleStats : MonoBehaviour
{
    [SerializeField] VehicleDefinition definition;

    // ------------- Immutable “read-only” -------------
    public VehicleDefinition Def => definition;

    // ------------- Mutable per-instance --------------
    public string vehicleName = "Vehicle";
    public bool IsBusy { get; protected set; }
    public float moveSpeed = 2f;
    public float fuel = 100f;
    public float durability = 100f;
    public float price = 100f;
    public float capacity = 100f;

    void Awake()
    {
        vehicleName = definition.DisplayName;
        moveSpeed = definition.MoveSpeed;
        capacity = definition.Capacity;
        price = definition.Price;
    }

    public void SetBusy(bool value)
    {
        IsBusy = value;
    }
}
