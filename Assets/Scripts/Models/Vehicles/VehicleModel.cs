using UnityEngine;
public abstract class VehicleModel
{
    public string Id;
    public string PrefabGuid;
    public string DisplayName;
    public Sprite Icon;
    public UnityEngine.Vector3 LoadedPosition;
    public UnityEngine.Quaternion LoadedRotation;

    public float MoveSpeed;
    public float Fuel;
    public float Durability;
    public float Price;
    public float Capacity;

    public bool IsBusy;
    public VehicleType Type;
    public string HomeId;
    public int CurrentTileIndex;
}