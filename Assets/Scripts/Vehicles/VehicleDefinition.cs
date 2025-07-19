using UnityEngine;

[CreateAssetMenu(fileName = "VehicleDefinition", menuName = "Roast/Vehicles/VehicleDefinition")]
public class VehicleDefinition : ScriptableObject
{
    [field: Header("Vehicle Definition")]
    [field: SerializeField] public string PrefabGuid { get; private set; }
    [field: SerializeField] public string DisplayName { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }

    [field: Header("Vehicle Stats")]
    [field: SerializeField] public float MoveSpeed { get; private set; }
    [field: SerializeField] public float Fuel { get; private set; }
    [field: SerializeField] public float Durability { get; private set; }
    [field: SerializeField] public float Price { get; private set; }
    [field: SerializeField] public float Capacity { get; private set; }

    [field: Header("Vehicle State")]
    [field: SerializeField] public VehicleType Type { get; private set; }
}