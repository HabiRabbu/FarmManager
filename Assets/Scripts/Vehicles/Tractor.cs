using UnityEngine;
using Harvey.Farm.FieldScripts;
using System.Collections;
using System.Collections.Generic;

namespace Harvey.Farm.VehicleScripts
{
    public class Tractor : Vehicle
    {
        public override void StartTask(FieldTile startTile)
        {
            if (IsBusy) return;
            StartCoroutine(StartPlowing(startTile));
        }

        private IEnumerator StartPlowing(FieldTile startTile)
        {
            IsBusy = true;

            var field = startTile.GetComponentInParent<Field>();
            var serp = field.GetSerpentineTiles();

            var waypoints = new List<Vector3>(serp.Length);
            foreach (var t in serp)
                waypoints.Add(t.WorldPosition);

            yield return MoveAlong(waypoints, i =>
            {
                if (!serp[i].IsPlowed)
                    serp[i].Plow();
            });

            IsBusy = false;
        }

    }
}
