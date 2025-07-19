using UnityEngine;

namespace Harvey.Data.Fields
{
    [System.Serializable]
    public struct FieldSaveData
    {
        public string Id;
        public string DisplayName;
        public string PrefabGuid;
        public Vector3 Position;
        public int Width, Height;
        public float TileSize;
        public int CurrentState;
        public string CurrentCropId;
        public string TileFlags;
    }
}