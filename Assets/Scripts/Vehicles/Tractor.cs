using UnityEngine;
using Harvey.Farm.FieldScripts;
using System.Collections;
using System.Collections.Generic;
using Harvey.Farm.JobScripts;
using Harvey.Farm.Events;
using Harvey.Farm.Implements;
using Harvey.Farm.Buildings;
using System;
using Unity.VisualScripting;
using System.Linq;

namespace Harvey.Farm.VehicleScripts
{
    public class Tractor : Vehicle
    {

        [Header("Hitch")]
        [SerializeField] Transform hitchPoint;

        [Header("Lookup")]
        [SerializeField] ImplementLibrary implementLibrary;

        ImplementBehaviour currentImplement;

        public override bool CanDo(JobType type) =>
            (type == JobType.Plow || type == JobType.Seed)
            && !IsBusy
            && Def.Type == VehicleType.Tractor;

        public override void StartTask(FieldJob job)
        {
            if (IsBusy || !CanDo(job.Type)) return;

            SetBusy(true);
            CurrentField = job.Field;
            StartCoroutine(StartJob(job));
        }

        IEnumerator StartJob(FieldJob job)
        {
            do
            {
                /* ---------- 1. fetch tool ---------- */
                if (!currentImplement || currentImplement.Def.Job != job.Type)
                    yield return StartCoroutine(FetchTool(job));

                /* ---------- 2. notify field ---------- */
                var field = job.Field;
                field.Begin(job.Type, job.Crop ? job.Crop : null);

                /* ---------- 3. drive serpentine ---------- */
                var serp = field.GetSerpentineTiles();
                var waypoints = new List<Vector3>(serp.Length);
                foreach (var t in serp) waypoints.Add(t.WorldPosition);

                Action<int> perTile = job.Type switch
                {
                    JobType.Plow => i => { var tile = serp[i]; if (!tile.IsPlowed) tile.Plow(); }
                    ,
                    JobType.Seed => i => { var tile = serp[i]; if (!tile.IsSeeded) tile.Seed(field.currentCrop); }
                    ,
                    _ => null
                };

                yield return MoveAlong(waypoints, perTile);

                /* ---------- 4. return tool ---------- */
                yield return StartCoroutine(ReturnTool());

                SetBusy(false);
                CurrentField = null;
                yield return null;
            }
            while (JobQueue.TryDequeue(out job));
        }

        /* ------------------------------------------------------------ */
        IEnumerator FetchTool(FieldJob job)
        {
            var shed = BuildingManager.Instance.GetNearestShed(transform.position);
            if (!shed) yield break;

            yield return MoveTo(shed.transform.position);
            ImplementBehaviour implement;
            shed.TryCheckoutByID(job.ToolId, out implement);
            currentImplement = implement;
            if (currentImplement) currentImplement.AttachTo(hitchPoint);
        }

        IEnumerator ReturnTool()
        {
            if (!currentImplement) yield break;
            var shed = BuildingManager.Instance.GetNearestShed(transform.position);
            yield return MoveTo(shed.transform.position);
            shed.ReturnImplement(currentImplement);
            currentImplement = null;
        }

        /* tiny helper – straight-line move */
        IEnumerator MoveTo(Vector3 target)
        {
            float dist = Vector3.Distance(transform.position, target);
            yield return MoveAlong(new List<Vector3> { target }, null);
        }

    }
}