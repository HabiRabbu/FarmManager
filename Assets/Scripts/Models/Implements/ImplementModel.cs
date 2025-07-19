using UnityEngine;

[System.Serializable]
public class ImplementModel
{
    //Name/Visual/Etc
    public string Id;
    public string PrefabGuid;
    public string DisplayName;
    public Sprite Icon;

    //Inner workings
    public bool IsReserved;
    public ImplementType Type;
    public JobType Job;
    public string CurrentParentId;
    public int AnchorIndex;


    //Stats
    public float Durability;
    public float Price;

    //Loaded/Cached info
    public Vector3 LoadedPosition;
    public Quaternion LoadedRotation;
    public string HomeId;

}