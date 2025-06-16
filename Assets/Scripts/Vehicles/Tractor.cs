using UnityEngine;
using Harvey.Farm.FieldScripts;
using System.Collections;
using System.Collections.Generic;

namespace Harvey.Farm.VehicleScripts
{
    public class Tractor : Vehicle
    {

        public override bool CanDo(JobType type) => type == JobType.Plow; //Tractor plows (and only plows) for now.

        public override void StartTask(FieldJob job)
        {
            if (IsBusy || !CanDo(job.Type)) return;

            CurrentField = job.Field;
            StartCoroutine(StartJob(job));
        }

        private IEnumerator StartJob(FieldJob job)
        {
            SetBusy(true);

            var field = job.Field;
            var serp = field.GetSerpentineTiles();

            var waypoints = new List<Vector3>(serp.Length);
            foreach (var t in serp)
                waypoints.Add(t.WorldPosition);

            yield return MoveAlong(waypoints, i =>
            {
                FieldTile tile = serp[i];

                if (job.Type == JobType.Plow && !tile.IsPlowed)
                    tile.Plow();
                // later: else-if Seed/Harvest …
            });

            SetBusy(false);
            CurrentField = null;
        }

    }
}
