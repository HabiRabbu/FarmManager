using UnityEngine;

namespace Harvey.Data.Buildings
{
    [System.Serializable]
    public struct ShedSaveData
    {
        public string Id;
        public string DisplayName;
        public string PrefabGuid;
        public Vector3 Position;
        public Quaternion Rotation;

        public BuildingType Type;
        public int Capacity;
        public int Price;
    }
}