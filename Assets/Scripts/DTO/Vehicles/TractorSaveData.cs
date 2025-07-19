namespace Harvey.Data.Vehicles
{
    [System.Serializable]
    public class TractorSaveData
    {
        public string Id;
        public string PrefabGuid;
        public string DisplayName;
        public UnityEngine.Vector3 Position;
        public UnityEngine.Quaternion Rotation;

        public float MoveSpeed;
        public float Fuel;
        public float Durability;
        public float Price;
        public float Capacity;

        public bool IsBusy;
        public VehicleType Type;
        public string HomeId;
        public int CurrentTileIndex;
        public string AttachedToolId;
    }
}