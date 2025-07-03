using UnityEngine;

[CreateAssetMenu(fileName = "ShedDefinition", menuName = "Roast/Buildings/ShedDefinition")]
public class ShedDefinition : BuildingDefinition
{
    [field: Header("Shed Capacity")]
    [field: SerializeField] public int ImplementSlots { get; private set; }

    [field: Header("Initial Implements")]
    [field: SerializeField] public ImplementDefinition[] Preload { get; private set; }

}