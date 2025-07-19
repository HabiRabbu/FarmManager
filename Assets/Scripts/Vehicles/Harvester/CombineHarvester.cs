using UnityEngine;
using Harvey.Farm.Fields;
using Harvey.Farm.Jobs;
using System.Collections;
using System.Collections.Generic;

namespace Harvey.Farm.VehicleScripts
{
    public class CombineHarvester : Vehicle
    {

        HarvesterRunner _runner;

        public HarvesterModel HarvesterModel => _stats.Model as HarvesterModel;

        protected override void Awake()
        {
            base.Awake();
            _runner = GetComponent<HarvesterRunner>();
        }

        public override bool CanDo(JobType type) =>
            (type is JobType.Harvest) && !HarvesterModel.IsBusy && HarvesterModel.Type == VehicleType.CombineHarvester;

        public override void StartTask(FieldJob job, int resumeTile = 0)
        {
            if (_stats.IsBusy || !CanDo(job.Type)) return;

            SetBusy(true);
            _stats.CurrentField = job.Field;
            _runner.Run(job, resumeTile);
        }
    }
}
