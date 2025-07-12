using System.Collections;
using System.Collections.Generic;
using Harvey.Farm.Jobs;
using Harvey.Farm.Fields;
using UnityEngine;
using Harvey.Farm.VehicleScripts;
using Harvey.Farm.Buildings;
using Microsoft.Unity.VisualStudio.Editor;

namespace Harvey.Farm.Workers
{
    public class Worker : MonoBehaviour, IJobAgent
    {
        public Queue<FieldJob> JobQueue { get; } = new();

        private WorkerStats _stats;
        private WorkerMover _mover;
        private WorkerJobRunner _runner;
        //ImplementHandler _tools;     //TODO: Maybe in future if workers use tools? Shared but with an if to see where they get them from idk

        public bool IsBusy => _stats.IsBusy;
        public string DisplayName => _stats.WorkerName;
        public Sprite Portrait => _stats.Def.Portrait;
        public string Id => _stats.GetId();
        public bool IsInitialised { get; private set; }

        public HouseBuilding Home => _stats.GetHome();
        public void SetHome(HouseBuilding home) => _stats.SetHome(home);

        void Awake()
        {
            _stats = GetComponent<WorkerStats>();
            _mover = GetComponent<WorkerMover>();
            _runner = GetComponent<WorkerJobRunner>();
            //_tools  = GetComponent<ImplementHandler>();
            WorkerManager.Instance.Register(this);
        }

        public void Init(WorkerDefinition def, HouseBuilding home)
        {
            // cache refs
            if (!_stats)
                _stats = GetComponent<WorkerStats>() ?? gameObject.AddComponent<WorkerStats>();
            if (!_mover)
                _mover = GetComponent<WorkerMover>() ?? gameObject.AddComponent<WorkerMover>();
            if (!_runner)
                _runner = GetComponent<WorkerJobRunner>() ?? gameObject.AddComponent<WorkerJobRunner>();
            //_tools = GetComponent<ImplementHandler>();

            _stats.InjectDefinition(def);
            SetHome(home);

            IsInitialised = true;
        }

        public void ReturnHome()
        {
            Home.ReturnWorker(this);
        }

        public bool CanDo(JobType type) =>
            (type is JobType.Plow or JobType.Seed or JobType.Harvest) && !IsBusy;

        public void Enqueue(FieldJob job) => JobQueue.Enqueue(job);

        public void StartTask(FieldJob job)
        {
            if (!gameObject.activeInHierarchy)
                Home.DeployWorker(this);

            if (_stats.IsBusy || !CanDo(job.Type)) return;

            _stats.SetBusy(true);
            _runner.Run(job);
        }

    }

}
