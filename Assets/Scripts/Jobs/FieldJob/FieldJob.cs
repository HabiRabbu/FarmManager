using Harvey.Data.Coffee;
using Harvey.Farm.Fields;

namespace Harvey.Farm.Jobs
{
    public readonly struct FieldJob
    {
        public readonly FieldController Field;
        public readonly JobType Type;
        public readonly CoffeeCropData Crop;
        public readonly string ToolId;

        public FieldJob(FieldController field,
                   JobType type,
                   CoffeeCropData crop = null,
                    string toolId = null)
        {
            Field = field;
            Type = type;
            Crop = crop;
            ToolId = toolId;
        }
    }
}