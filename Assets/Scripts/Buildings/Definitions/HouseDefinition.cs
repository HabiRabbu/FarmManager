using Harvey.Farm.Workers;
using UnityEngine;

[CreateAssetMenu(fileName = "HouseDefinition", menuName = "Roast/Buildings/HouseDefinition")]
public class HouseDefinition : BuildingDefinition
{
    [field: Header("Initial Workers")]
    [field: SerializeField] public WorkerDefinition[] Preload { get; private set; }
}