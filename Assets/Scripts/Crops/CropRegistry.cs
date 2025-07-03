using System.Collections.Generic;
using UnityEngine;

namespace Harvey.Farm.Crops
{
    [CreateAssetMenu(fileName = "CropRegistry", menuName = "Roast/Crops/CropRegistry")]
    public class CropRegistry : ScriptableObject
    {
        public List<CropDefinition> crops = new();

        public CropDefinition GetByName(string name) =>
            crops.Find(c => c.cropName == name);
    }
}
