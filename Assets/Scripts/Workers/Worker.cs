using System.Collections;
using Harvey.Farm.Jobs;
using Harvey.Farm.Buildings;
using Harvey.Farm.Events;
using UnityEngine;

namespace Harvey.Farm.Workers
{
    public class Worker : MonoBehaviour, IJobAgent
    {
        public FieldController CurrentField => _stats.CurrentField;

        public string GetId() => _stats.GetId();
        public int CurrentTileIndex => _stats.CurrentTileIndex;

        public GameObject Meshes;
        private WorkerStats _stats;
        private WorkerMover _mover;
        private JobExecutor _executor;
        private WorkerBrain _brain;
        //ImplementHandler _tools;     //TODO: Maybe in future if workers use tools? Shared but with an if to see where they get them from idk

        // Component Getters
        public WorkerStats Stats => _stats;
        public WorkerModel Model => _stats.Model;
        public WorkerMover Mover => _mover;
        public JobExecutor Executor => _executor;
        public WorkerBrain Brain => _brain;

        // Helper Getters
        public Vector3 HomePosition => BuildingManager.Instance.GetHomePosition(_stats.Model.HomeId);
        public string GetHomeName() => BuildingManager.Instance.GetHomeNameById(_stats.Model.HomeId);

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
            _executor = GetComponent<JobExecutor>();
            _brain = GetComponent<WorkerBrain>();
            //_tools  = GetComponent<ImplementHandler>();
        }

        public void ReturnHome()
        {
            Debug.Log($"Worker {DisplayName} returning home to {GetHomeName()}");
            ResetWorker();
            StartCoroutine(ReturnHomeRoutine());
        }

        IEnumerator ReturnHomeRoutine()
        {
            yield return _mover.MoveTo(HomePosition);

            SetBusy(false);
            Debug.Log($"Worker {DisplayName} has returned home to {GetHomeName()} at {HomePosition}");
            yield return null; // Wait one frame
            Home.ReturnWorker(this);
        }

        public bool CanDo(JobType type) =>
            (type is JobType.Plow or JobType.Seed or JobType.Harvest) && !IsBusy;

        public void StartTask(IJob job, int resumeTile = 0)
        {
            if (!gameObject.activeInHierarchy)
                Home.DeployWorker(this);

            if (_stats.IsBusy) return;

            _stats.SetBusy(true);
            //_stats.CurrentField = job.Field;
            _executor.StartJob(job);
        }

        public void ResetWorker()
        {
            gameObject.SetActive(true);
            Meshes.SetActive(true);
            transform.SetParent(null);
            this.StopAllCoroutines();
            _mover.StopAllTweens();
        }
    }
}
