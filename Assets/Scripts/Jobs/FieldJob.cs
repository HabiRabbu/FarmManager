using Harvey.Farm.Crops;
using Harvey.Farm.Fields;

namespace Harvey.Farm.JobScripts
{
    public readonly struct FieldJob
    {
        public readonly FieldController Field;
        public readonly JobType Type;
        public readonly CropDefinition Crop;
        public readonly string ToolId;

        public FieldJob(FieldController field,
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