using UnityEngine;

[System.Serializable]
public class FieldModel
{
    // identity / placement
    public string Id;
    public string DisplayName;
    public string PrefabGuid;
    public Vector3 Position;

    // grid
    public int Width;
    public int Height;
    public float TileSize;

    // gameplay
    public int CurrentState;
    public string CurrentCropId;

    // per-tile flags packed as “. P S H”
    public string TileFlags;
}
