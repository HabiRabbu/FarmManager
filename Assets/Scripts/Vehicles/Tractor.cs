using UnityEngine;
using Harvey.Farm.FieldScripts;
using System.Collections;
using System.Collections.Generic;
using Harvey.Farm.JobScripts;

namespace Harvey.Farm.VehicleScripts
{
    public class Tractor : Vehicle
    {

        public override bool CanDo(JobType type) =>
            type == JobType.Plow && !IsBusy && Def.Type == VehicleType.Tractor;

        public override void StartTask(FieldJob job)
        {
            if (IsBusy || !CanDo(job.Type)) return;

            CurrentField = job.Field;
            StartCoroutine(StartJob(job));
        }

        private IEnumerator StartJob(FieldJob job)
        {
            do
            {
                SetBusy(true);
                var field = job.Field;
                field.Begin(job.Type);

                var serp = field.GetSerpentineTiles();
                var waypoints = new List<Vector3>(serp.Length);
                foreach (var t in serp) waypoints.Add(t.WorldPosition);

                yield return MoveAlong(waypoints, i =>
                {
                    FieldTile tile = serp[i];
                    if (job.Type == JobType.Plow && !tile.IsPlowed) tile.Plow();
                });

                SetBusy(false);
                CurrentField = null;
            }
            while (JobQueue.TryDequeue(out job));
        }

    }
}
