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
    //ImplementHandler _tools;     //TODO: Maybe in future if workers use tools? Shared but with an if to see where they get them from idk
    HouseBuilding _home;

    public bool IsBusy => _stats.IsBusy;
    public string DisplayName => _stats.WorkerName;
    public Sprite Portrait => _stats.Def.Portrait;
    public bool IsInitialised { get; private set; }

    void Awake()
    {
        _stats = GetComponent<WorkerStats>();
        _mover = GetComponent<WorkerMover>();
        //_tools  = GetComponent<ImplementHandler>();
        WorkerManager.Instance.Register(this);
    }

    public bool CanDo(JobType type) =>
        (type is JobType.Plow or JobType.Seed or JobType.Harvest) && !IsBusy;

    public void Enqueue(FieldJob job) => JobQueue.Enqueue(job);

    public void StartTask(FieldJob job)
    {
        //If not active / aka out of the house and in scene
        if (!gameObject.activeInHierarchy)
            _home?.DeployWorker(this);

        if (IsBusy || !CanDo(job.Type)) return;

        _stats.SetBusy(true);
        StartCoroutine(DoFieldJob(job));
    }

    public void SetHome(HouseBuilding house) => _home = house;

    public void Init(WorkerDefinition def, HouseBuilding home)
    {
        // cache refs
        if (!_stats)
            _stats = GetComponent<WorkerStats>() ?? gameObject.AddComponent<WorkerStats>();
        if (!_mover)
            _mover = GetComponent<WorkerMover>() ?? gameObject.AddComponent<WorkerMover>();
        //_tools = GetComponent<ImplementHandler>();

        _stats.InjectDefinition(def);
        _home = home;

        IsInitialised = true;
    }

    public void ReturnHome()
    {
        _home.Reclaim(this);
    }

    IEnumerator DoFieldJob(FieldJob job)
    {
        var field = job.Field;
        field.BeginJob(job.Type, job.Crop);

        while (true)
        {
            FieldTile target = field.GetNearestAvailableTile(job.Type, transform.position);
            if (target == null) break;

            yield return _mover.MoveTo(target.WorldPosition);

            float workTime = _stats.GetActionDuration(job.Type);
            if (workTime > 0f) yield return new WaitForSeconds(workTime);

            switch (job.Type)
            {
                case JobType.Plow: target.Plow(); break;
                case JobType.Seed: target.Seed(field.currentCrop); break;
                case JobType.Harvest: target.Harvest(); break;
            }

            target.ClearReservation();
            yield return null;
        }

        _stats.SetBusy(false);
        _home.Reclaim(this);

        // dispatch next queued job
        if (JobQueue.TryDequeue(out var next))
            StartTask(next);
    }
}

}
