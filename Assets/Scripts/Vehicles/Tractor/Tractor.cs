using UnityEngine;
using Harvey.Farm.Fields;
using System.Collections;
using System.Collections.Generic;
using Harvey.Farm.Events;
using Harvey.Farm.Implements;
using Harvey.Farm.Buildings;
using System;
using Unity.VisualScripting;
using System.Linq;
using NUnit.Framework;
using Harvey.Farm.Jobs;

namespace Harvey.Farm.VehicleScripts
{
    public class Tractor : Vehicle
    {
        TractorJobRunner _runner;

        public VehicleStats Stats => _stats;
        public TractorModel TractorModel => _stats.Model as TractorModel;

        public string AttachedToolId => TractorModel.AttachedToolId;

        protected override void Awake()
        {
            base.Awake();
            _runner = GetComponent<TractorJobRunner>();
        }

        public override bool CanDo(JobType type) =>
            (type is JobType.Plow or JobType.Seed) && _stats.Model.IsBusy == false && TractorModel.Type == VehicleType.Tractor;

        public override void StartTask(FieldJob job, int resumeTile = 0)
        {
            if (_stats.IsBusy || !CanDo(job.Type)) return;

            SetBusy(true);
            _stats.CurrentField = job.Field;
            _runner.Run(job, resumeTile);
            TractorModel.AttachedToolId = job.ToolId;
        }
    }

}