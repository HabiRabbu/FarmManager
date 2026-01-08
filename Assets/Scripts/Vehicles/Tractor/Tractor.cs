using Harvey.Farm.Jobs;

namespace Harvey.Farm.VehicleScripts
{
    public class Tractor : Vehicle
    {
        public TractorModel TractorModel => _stats.Model as TractorModel;
        public string AttachedToolId => TractorModel.AttachedToolId;

        public override bool CanDo(JobType type) =>
            (type is JobType.Plow or JobType.Seed) && _stats.Model.IsBusy == false && TractorModel.Type == VehicleType.Tractor;
    }
}