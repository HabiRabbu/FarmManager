using Harvey.Data.Coffee;
using Harvey.Farm.Fields;

namespace Harvey.Farm.Jobs.VehicleField
{
    /// <summary>Design-time description for vehicle field jobs (no progress).</summary>
    public readonly struct VehicleFieldJob
    {
        public readonly FieldController Field;
        public readonly JobType Type;
        public readonly CoffeeCropData Crop;
        public readonly string VehicleId;
        public readonly string ImplementId;

        public VehicleFieldJob(FieldController field,
                               JobType type,
                               CoffeeCropData crop,
                               string vehicleId,
                               string implementId)
        {
            Field = field;
            Type = type;
            Crop = crop;
            VehicleId = vehicleId;
            ImplementId = implementId;
        }
    }
}