using System;

namespace Harvey.Data.Jobs
{
    [Serializable]
    public struct ActiveJobSaveData
    {
        public string AgentId;
        public string FieldId;
        public JobType Type;
        public string CropId;
        public string ToolId;
        public int NextTileIndex;
        public int TilesCompleted; // Optional, for tracking progress
    }
}