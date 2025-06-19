using UnityEngine;
using Harvey.Farm.FieldScripts;
using System.Collections;
using System.Collections.Generic;
using Harvey.Farm.JobScripts;
using Harvey.Farm.Events;

namespace Harvey.Farm.VehicleScripts
{
    public class Tractor : Vehicle
    {

        public override bool CanDo(JobType type) =>
            (type == JobType.Plow || type == JobType.Seed)
            && !IsBusy
            && Def.Type == VehicleType.Tractor;

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

                if (job.Type == JobType.Seed)
                    field.Begin(job.Type, job.Crop);
                else
                    field.Begin(job.Type);

                var serp = field.GetSerpentineTiles();
                var waypoints = new List<Vector3>(serp.Length);
                foreach (var t in serp) waypoints.Add(t.WorldPosition);

                yield return MoveAlong(waypoints, i =>
                {
                    FieldTile tile = serp[i];

                    switch (job.Type)
                    {
                        case JobType.Plow:
                            if (!tile.IsPlowed) tile.Plow();
                            break;

                        case JobType.Seed:
                            if (!tile.IsSeeded) tile.Seed(field.currentCrop);
                            break;
                    }
                });

                SetBusy(false);
                CurrentField = null;
            }
            while (JobQueue.TryDequeue(out job));
        }

    }
}
