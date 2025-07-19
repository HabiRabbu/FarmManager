using UnityEngine;

namespace Harvey.Data.Implements
{
    [System.Serializable]
    public struct ImplementSaveData
    {
        //Name/Visual/Etc
        public string Id;
        public string PrefabGuid;
        public string DisplayName;
        public string IconGuid; //Different in Model

        //Inner workings
        public Vector3 Position; //Different in Model
        public Quaternion Rotation; //Different in Model
        public bool IsReserved;
        public ImplementType Type;
        public JobType Job;
        public string CurrentParentId;
        public string HomeId; //Not in Model
        public int AnchorIndex;


        //Stats
        public float Durability;
        public float Price;
    }
}