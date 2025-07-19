using UnityEngine;

[CreateAssetMenu(fileName = "ImplementDefinition", menuName = "Roast/Implements/ImplementDefinition")]
public class ImplementDefinition : ScriptableObject
{
    [field: Header("Implement Definition")]
    //Name/Visual/Etc
    public string Id;
    [field: SerializeField] public string PrefabGuid { get; private set; }
    [field: SerializeField] public string DisplayName { get; private set; }

    //Inner workings
    [field: SerializeField] public ImplementType Type { get; private set; }
    [field: SerializeField] public JobType Job { get; private set; }

    //Stats
    [field: SerializeField] public float Durability { get; private set; }
    [field: SerializeField] public float Price { get; private set; }
}