// Assets/Scripts/Data/Fields/FieldSaveData.cs
namespace Harvey.Data.Fields
{
    [System.Serializable]
    public class FieldSaveData
    {
        public string Id;
        public int Width;
        public int   Height;
        public float TileSize;
        public string PrefabGuid;
        public string TilePrefabGuid;
        public UnityEngine.Vector3 Position;
        public int    CurrentState;
        public string CurrentCropId;
        public string TileFlags;        // '.' / 'P' / 'S' / 'H'
    }
}
