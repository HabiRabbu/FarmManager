using UnityEngine;
using Harvey.Farm.FieldScripts;
using Harvey.Farm.JobScripts;
using System.Collections;
using System.Collections.Generic;

namespace Harvey.Farm.VehicleScripts
{
    public class CombineHarvester : Vehicle
    {
        public override bool CanDo(JobType type) =>
            type == JobType.Harvest
            && !IsBusy
            && Def.Type == VehicleType.CombineHarvester;

        public override void StartTask(FieldJob job)
        {
            if (IsBusy || !CanDo(job.Type)) return;

            CurrentField = job.Field;
            StartCoroutine(HarvestRoutine(job));
        }

        IEnumerator HarvestRoutine(FieldJob job)
        {
            SetBusy(true);
            var field = job.Field;
            field.Begin(JobType.Harvest);

            var serp = field.GetSerpentineTiles();
            var waypoints = new List<Vector3>(serp.Length);
            foreach (var t in serp) waypoints.Add(t.WorldPosition);

            yield return MoveAlong(waypoints, i =>
            {
                FieldTile tile = serp[i];
                if (!tile.IsHarvested) tile.Harvest();
            });

            SetBusy(false);
            CurrentField = null;
            // queue next job if any
            if (JobQueue.TryDequeue(out var next)) StartTask(next);
        }
    }
}
