using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Harvey.Farm.Buildings;
using Harvey.Farm.Implements;
using Harvey.Farm.JobScripts;
using Harvey.Farm.VehicleScripts;
using UnityEngine;

public class TractorJobRunner : MonoBehaviour
{
    ImplementHandler _tools;
    Vehicle _vehicle;
    Mover _mover;

    void Awake()
    {
        _tools = GetComponent<ImplementHandler>();
        _vehicle = GetComponent<Vehicle>();
        _mover = GetComponent<Mover>();
    }

    public void Run(FieldJob job) => StartCoroutine(RunJobs(job));

    IEnumerator RunJobs(FieldJob job)
    {
        do
        {
            /* 1. tool */
            if (!_tools.Has(job.Type))
                yield return _tools.Fetch(job.Type, job.ToolId);

            /* 2. field prep */
            var field = job.Field;
            field.Begin(job.Type, job.Crop);

            /* 3. serpentine drive */
            var serp = field.GetSerpentineTiles();
                var waypoints = new List<Vector3>(serp.Length);
                foreach (var t in serp) waypoints.Add(t.WorldPosition);

            System.Action<int> perTile = job.Type switch
            {
                JobType.Plow => i => { var tile = serp[i]; if (!tile.IsPlowed) tile.Plow(); }
                ,
                JobType.Seed => i => { var tile = serp[i]; if (!tile.IsSeeded) tile.Seed(field.currentCrop); }
                ,
                _ => null
            };

            yield return _mover.MoveAlong(waypoints, perTile);

            /* 4. clean-up */
            yield return _tools.Return();
        }
        while (_vehicle.JobQueue.TryDequeue(out job));

        _vehicle.SetBusy(false);
    }
}
