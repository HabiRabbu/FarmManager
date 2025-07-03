using UnityEngine;

/// <summary>Data-only base for all buildings.</summary>
public abstract class BuildingDefinition : ScriptableObject
{
    [field: SerializeField] public string DisplayName { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public float Price { get; private set; }
    [field: SerializeField] public BuildingType Type { get; private set; }
    [field: SerializeField] public GameObject ModelPrefab { get; private set; }
}
