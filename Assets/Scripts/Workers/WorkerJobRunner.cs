using System.Collections;
using System.Collections.Generic;
using Harvey.Farm.Fields;
using Harvey.Farm.Implements;
using Harvey.Farm.Jobs;
using Harvey.Farm.Movement;
using Harvey.Farm.Workers;
using UnityEngine;

namespace Harvey.Farm.Workers
{
    [RequireComponent(typeof(WorkerMover))]
    public class WorkerJobRunner : MonoBehaviour
    {
        Worker          _worker;
        WorkerStats     _stats;
        WorkerMover     _mover;
        //ImplementHandler _tools;   // uncomment when workers use tools

        void Awake()
        {
            _worker = GetComponent<Worker>();
            _stats  = GetComponent<WorkerStats>();
            _mover  = GetComponent<WorkerMover>();
            //_tools  = GetComponent<ImplementHandler>();
        }

        public void Run(FieldJob job) => StartCoroutine(RunJobs(job));

        IEnumerator RunJobs(FieldJob job)
        {
            do
            {
                /* -------- 1. tool fetch (future) --------
                if (!_tools.Has(job.Type))
                    yield return _tools.Fetch(job.Type, job.ToolId);
                */

                /* -------- 2. field prep -------- */
                var field = job.Field;
                field.BeginJob(job.Type, job.Crop);

                /* -------- 3. tile-by-tile work -------- */
                while (true)
                {
                    FieldTile tile = field.GetNearestAvailableTile(job.Type, transform.position);
                    if (tile == null) break;            // field done

                    yield return _mover.MoveTo(tile.WorldPosition);

                    float t = _stats.GetActionDuration(job.Type);
                    if (t > 0f) yield return new WaitForSeconds(t);

                    switch (job.Type)
                    {
                        case JobType.Plow:    tile.Plow();                        break;
                        case JobType.Seed:    tile.Seed(field.currentCrop);       break;
                        case JobType.Harvest: tile.Harvest();                     break;
                    }

                    tile.ClearReservation();
                }

                /* -------- 4. tool return (future) --------
                yield return _tools.Return();
                */

                /* -------- 5. go home -------- */
                yield return _mover.ReturnToHome();
                _stats.CurrentField = null;
                _stats.SetCurrentTileIndex(0);
                _worker.ReturnHome();          // queues back in the house

            } while (_worker.JobQueue.TryDequeue(out job));

            _stats.SetBusy(false);
        }
    }
}
