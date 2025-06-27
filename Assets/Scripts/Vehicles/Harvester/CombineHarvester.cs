using UnityEngine;
using Harvey.Farm.FieldScripts;
using Harvey.Farm.JobScripts;
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
            (type is JobType.Harvest) && !Stats.IsBusy && Stats.Def.Type == VehicleType.CombineHarvester;

        public override void StartTask(FieldJob job)
        {
            if (Stats.IsBusy || !CanDo(job.Type)) return;

            SetBusy(true);
            CurrentField = job.Field;
            _runner.Run(job);
        }
    }
}
