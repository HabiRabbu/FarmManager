using UnityEngine;

namespace Harvey.Farm.Crops
{
    /// <summary>
    /// Represents a crop definition with its properties and growth stages.
    /// </summary>
    [CreateAssetMenu(fileName = "CropDefinition", menuName = "Roast/Crops/CropDefinition")]
    public class CropDefinition : ScriptableObject
    {
        [field: Header("Crop Definition")]
        public string cropName = "Wheat";
        public float growSeconds = 10f;
        public GameObject cropPrefab;
    }
}
