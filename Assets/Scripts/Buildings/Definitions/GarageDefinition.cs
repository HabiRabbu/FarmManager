using UnityEngine;

[CreateAssetMenu(fileName = "GarageDefinition",
                 menuName = "Roast/Buildings/GarageDefinition")]
public class GarageDefinition : BuildingDefinition
{
    [field: Header("Optional - Initial Vehicles")]
    [field: SerializeField] public VehicleDefinition[] Preload { get; private set; }
}
