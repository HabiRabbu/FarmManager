using UnityEngine;

public abstract class BuildingModel
{
    public string Id;
    public string DisplayName;
    public string PrefabGuid;
    public Vector3 LoadedPosition;
    public Quaternion LoadedRotation;

    public BuildingType Type;
    public int Capacity;

    public int Price;
    // Etc etc
}