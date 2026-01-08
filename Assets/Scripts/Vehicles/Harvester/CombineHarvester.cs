using Harvey.Farm.Jobs;

namespace Harvey.Farm.VehicleScripts
{
    public class CombineHarvester : Vehicle
    {
        public HarvesterModel HarvesterModel => _stats.Model as HarvesterModel;

        public override bool CanDo(JobType type) =>
            (type is JobType.Harvest) && !HarvesterModel.IsBusy && HarvesterModel.Type == VehicleType.CombineHarvester;
    }
}
