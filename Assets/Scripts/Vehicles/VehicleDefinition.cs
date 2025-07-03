using UnityEngine;

[CreateAssetMenu(fileName = "VehicleDefinition", menuName = "Farm/Vehicles/VehicleDefinition")]
public class VehicleDefinition : ScriptableObject
{
    [field: Header("Vehicle Definition")]
    [field: SerializeField] public string DisplayName { get; private set; }
    [field: SerializeField] public VehicleType Type { get; private set; }
    [field: SerializeField] public GameObject ModelPrefab { get; private set; }
    [field: SerializeField] public float MoveSpeed { get; private set; }
    [field: SerializeField] public float Capacity { get; private set; }
    [field: SerializeField] public float Price { get; private set; }
}