[System.Serializable]
public class WorkerModel
{
    public string Id;
    public string PrefabGuid;
    public string PortraitGuid;
    public string DisplayName;
    public UnityEngine.Vector3 LoadedPosition;
    public UnityEngine.Quaternion LoadedRotation;

    public bool ActiveInScene;    // false → still inside a house
    public bool IsBusy;
    public string HomeId;
    public int CurrentTileIndex;

    public float WalkSpeed;
    public float plowSeconds;
    public float seedSeconds;
    public float harvestSeconds;
}
