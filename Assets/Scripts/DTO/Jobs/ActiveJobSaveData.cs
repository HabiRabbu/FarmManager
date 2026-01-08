using System;

using Harvey.Farm.Jobs;

namespace Harvey.Data.Jobs
{
    public enum JobSaveState { Pending, Active, Paused, WaitingForResources }

    [Serializable]
    public struct ActiveJobSaveData
    {
        public string AgentId;
        public string FieldId;
        public JobType Type;            // Plow/Seed/Harvest
        public string CropId;
        public string ToolId;

        public int ResumeToken;
        public int TilesCompleted;

        public bool IsVehicleJob;
        public string VehicleId;
        public string ImplementId;
        public AgentType RequiredAgent;
        public JobSaveState State;
        public bool IsOwnerJob;         // True = targeted to specific worker, False = open job
    }
}