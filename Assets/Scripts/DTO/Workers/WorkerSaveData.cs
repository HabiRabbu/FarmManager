namespace Harvey.Data.Workers
{
    [System.Serializable]
    public class WorkerSaveData
    {
        public string Id;
        public string PrefabGuid;
        public string PortraitGuid;
        public string DisplayName;
        public UnityEngine.Vector3 Position;
        public UnityEngine.Quaternion Rotation;

        public bool ActiveInScene;    // false → still inside a house
        public bool IsBusy;
        public string HomeId;
        public int CurrentTileIndex;

        //* ---------- Stats ---------- */
        public bool HasDrivingLicense = false;
        public float WalkSpeed;
        public float plowSeconds;
        public float seedSeconds;
        public float harvestSeconds;
    }
}