using UnityEngine;

namespace Harvey.Farm.Crops
{
    /// <summary>
    /// Represents a crop definition with its properties and growth stages.
    /// </summary>
    [CreateAssetMenu(fileName = "CropDefinition", menuName = "Scriptable Objects/CropDefinition")]
    public class CropDefinition : ScriptableObject
    {
        [field: Header("Crop Definition")]
        public string cropName = "Wheat";
        public float growSeconds = 10f;

        [Tooltip("0 = planted, 1 = mid-growth, 2 = ready")]
        public GameObject[] growthPrefabs = new GameObject[3];
    }
}
