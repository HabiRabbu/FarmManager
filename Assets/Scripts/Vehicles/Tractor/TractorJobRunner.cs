using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Harvey.Farm.Buildings;
using Harvey.Farm.Implements;
using Harvey.Farm.Jobs;
using Harvey.Farm.Movement;
using Harvey.Farm.VehicleScripts;

using UnityEngine;

public class TractorJobRunner : MonoBehaviour
{
    ImplementHandler _tools;
    Vehicle _vehicle;
    TractorMover _mover;

    void Awake()
    {
        _tools = GetComponent<ImplementHandler>();
        _vehicle = GetComponent<Vehicle>();
        _mover = GetComponent<TractorMover>();
    }

    public void Run(FieldJob job, int resumeTile) => StartCoroutine(RunJobs(job, resumeTile));

    IEnumerator RunJobs(FieldJob job, int resumeTile)
    {
        do
        {
            /* 1. tool */
            if (!_tools.Has(job.Type))
            {
                var tractor = (Tractor)_vehicle;
                if (tractor.AttachedToolId != null)
                {
                    _tools.CurrentImplement = ImplementManager.Instance.GetById(tractor.AttachedToolId);
                }
                else
                {
                    yield return _tools.Fetch(job.Type, job.ToolId);
                }
            }

            /* 2. field prep */
            var field = job.Field;
            field.BeginJob(job.Type, job.Crop);

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

            yield return _mover.MoveAlong(waypoints, perTile, resumeTile);

            /* 4. clean-up */
            var Tractor = _vehicle as Tractor;
            Tractor.TractorModel.AttachedToolId = null;
            yield return _tools.Return();
        }
        while (_vehicle.JobQueue.TryDequeue(out job));

        /* 5. Return Home */
        yield return _mover.ReturnToHome();
        _vehicle._stats.SetBusy(false);
        _vehicle._stats.CurrentField = null;
        _vehicle._stats.SetCurrentTileIndex(0);
        _vehicle.ReturnHome();
    }
}
