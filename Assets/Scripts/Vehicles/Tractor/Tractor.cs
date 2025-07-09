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

        protected override void Awake()
        {
            base.Awake();
            _runner = GetComponent<TractorJobRunner>();
        }

        protected override void Start()
        {
            base.Start();
        }

        public override bool CanDo(JobType type) =>
            (type is JobType.Plow or JobType.Seed) && !_stats.IsBusy && _stats.Def.Type == VehicleType.Tractor;

        public override void StartTask(FieldJob job)
        {
            if (_stats.IsBusy || !CanDo(job.Type)) return;

            SetBusy(true);
            CurrentField = job.Field;
            _runner.Run(job);
        }
    }

}