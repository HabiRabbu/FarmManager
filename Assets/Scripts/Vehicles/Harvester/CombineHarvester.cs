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

        protected override void Awake()
        {
            base.Awake();
            _runner = GetComponent<HarvesterRunner>();
        }

        public override bool CanDo(JobType type) =>
            (type is JobType.Harvest) && !_stats.IsBusy && _stats.Def.Type == VehicleType.CombineHarvester;

        public override void StartTask(FieldJob job)
        {
            if (_stats.IsBusy || !CanDo(job.Type)) return;

            SetBusy(true);
            CurrentField = job.Field;
            _runner.Run(job);
        }
    }
}
