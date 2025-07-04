using UnityEngine;

[CreateAssetMenu(fileName = "ImplementDefinition", menuName = "Scriptable Objects/ImplementDefinition")]
public class ImplementDefinition : ScriptableObject
{
    [field: Header("Implement Definition")]
    [field: SerializeField] public string DisplayName { get; private set; }
    [field: SerializeField] public ImplementType Type { get; private set; }
    [field: SerializeField] public JobType Job { get; private set; }
    [field: SerializeField] public GameObject ModelPrefab { get; private set; }
    [field: SerializeField] public float Price { get; private set; }
}