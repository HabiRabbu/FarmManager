using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Harvey.Farm.Buildings;
using Harvey.Farm.Implements;
using Harvey.Farm.JobScripts;
using Harvey.Farm.VehicleScripts;
using UnityEngine;

public class HarvesterRunner : MonoBehaviour
{
    Vehicle _vehicle;
    Mover _mover;

    void Awake()
    {
        _vehicle = GetComponent<Vehicle>();
        _mover = GetComponent<Mover>();
    }

    public void Run(FieldJob job) => StartCoroutine(RunJobs(job));

    IEnumerator RunJobs(FieldJob job)
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

            yield return _mover.MoveAlong(waypoints, perTile);
        }
        while (_vehicle.JobQueue.TryDequeue(out job));

        _vehicle.SetBusy(false);
    }
}
