using UnityEngine;

namespace Harvey.Farm.Workers
{
    [CreateAssetMenu(menuName = "Roast/Workers/Worker Definition")]
    public class WorkerDefinition : ScriptableObject
    {
        [field: Header("Identity")]
        [field: SerializeField] public string DisplayName { get; private set; }

        [field: Header("Stats")]
        [field: SerializeField] public float WalkSpeed = 1.8f;

        [Header("Base Action Durations (seconds)")]
        [field: SerializeField] public float plowSeconds = 6f;
        [field: SerializeField] public float seedSeconds = 2f;
        [field: SerializeField] public float harvestSeconds = 4f;

        [field: Header("Visual")]
        [field: SerializeField] public GameObject ModelPrefab;
        [field: SerializeField] public Sprite Portrait;
    }
}