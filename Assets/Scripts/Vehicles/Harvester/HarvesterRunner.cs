using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Harvey.Farm.Buildings;
using Harvey.Farm.Implements;
using Harvey.Farm.Jobs;
using Harvey.Farm.Movement;
using Harvey.Farm.VehicleScripts;

using UnityEngine;

public class HarvesterRunner : MonoBehaviour
{
    Vehicle _vehicle;
    TractorMover _mover;

    void Awake()
    {
        _vehicle = GetComponent<Vehicle>();
        _mover = GetComponent<TractorMover>();
    }

    public void Run(FieldJob job, int resumeTile) => StartCoroutine(RunJobs(job, resumeTile));

    IEnumerator RunJobs(FieldJob job, int resumeTile)
    {
        do
        {
            var field = job.Field;
            field.BeginJob(job.Type, job.Crop);

            var serp = field.GetSerpentineTiles();
            var waypoints = new List<Vector3>(serp.Length);
            foreach (var t in serp) waypoints.Add(t.WorldPosition);

            System.Action<int> perTile = job.Type switch
            {
                JobType.Harvest => i => { var tile = serp[i]; if (!tile.IsHarvested) tile.Harvest(); }
                ,
                _ => null
            };

            yield return _mover.MoveAlong(waypoints, perTile, resumeTile);
        }
        while (_vehicle.JobQueue.TryDequeue(out job));

        yield return _mover.ReturnToHome();
        _vehicle._stats.SetBusy(false);
        _vehicle._stats.SetCurrentTileIndex(0);
        _vehicle.ReturnHome();
    }
}
