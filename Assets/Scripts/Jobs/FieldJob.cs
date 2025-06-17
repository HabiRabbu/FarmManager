using Harvey.Farm.FieldScripts;

namespace Harvey.Farm.JobScripts
{
    public readonly struct FieldJob
    {
        public readonly Field Field;
        public readonly JobType Type;

        public FieldJob(Field field, JobType type)
        {
            Field = field;
            Type = type;
        }
    }
}