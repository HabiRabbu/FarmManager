using System.Collections;
using System.Collections.Generic;
using Harvey.Farm.Jobs;
using Harvey.Farm.Fields;
using UnityEngine;
using Harvey.Farm.VehicleScripts;
using Harvey.Farm.Buildings;
using Microsoft.Unity.VisualStudio.Editor;
using Harvey.Farm.Events;

namespace Harvey.Farm.Workers
{
    public class Worker : MonoBehaviour, IJobAgent
    {
        public Queue<FieldJob> JobQueue { get; } = new();
        public FieldController CurrentField => _stats.CurrentField;

        public string GetId() => _stats.GetId();
        public int CurrentTileIndex => _stats.CurrentTileIndex;

        private WorkerStats _stats;
        private WorkerMover _mover;
        private WorkerJobRunner _runner;
        //ImplementHandler _tools;     //TODO: Maybe in future if workers use tools? Shared but with an if to see where they get them from idk

        public WorkerStats Stats => _stats;
        public WorkerModel Model => _stats.Model;

        public bool IsBusy => _stats.IsBusy;
        public void SetBusy(bool value)
        {
            _stats.SetBusy(value);
            GameEvents.WorkerBusyChanged(this, value);
        }

        public string DisplayName => _stats.Model.DisplayName;

        public bool IsInitialised { get; private set; }

        public HouseBuilding Home => _stats.GetHome();
        public void SetHome(HouseBuilding home) => _stats.SetHome(home);

        void Awake()
        {
            _stats = GetComponent<WorkerStats>();
            _mover = GetComponent<WorkerMover>();
            _runner = GetComponent<WorkerJobRunner>();
            //_tools  = GetComponent<ImplementHandler>();
        }

        public void ReturnHome()
        {
            Home.ReturnWorker(this);
        }

        public bool CanDo(JobType type) =>
            (type is JobType.Plow or JobType.Seed or JobType.Harvest) && !IsBusy;

        public void Enqueue(FieldJob job) => JobQueue.Enqueue(job);

        public void StartTask(FieldJob job, int resumeTile = 0)
        {
            if (!gameObject.activeInHierarchy)
                Home.DeployWorker(this);

            if (_stats.IsBusy || !CanDo(job.Type)) return;

            _stats.SetBusy(true);
            _stats.CurrentField = job.Field;
            _runner.Run(job);
        }

    }

}
