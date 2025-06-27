using Harvey.Farm.Crops;
using Harvey.Farm.FieldScripts;

namespace Harvey.Farm.JobScripts
{
    public readonly struct FieldJob
    {
        public readonly Field Field;
        public readonly JobType Type;
        public readonly CropDefinition Crop;
        public readonly string ToolId;

        public FieldJob(Field field,
                   JobType type,
                   CropDefinition crop = null,
                    string toolId = null)
        {
            Field = field;
            Type = type;
            Crop = crop;
            ToolId = toolId;
        }
    }
}