using System.Collections.Generic;
using UnityEngine;
using Harvey.Farm.Events;
using Harvey.Farm.FieldScripts;
using Harvey.Farm.VehicleScripts;
using Harvey.Farm.Crops;

namespace Harvey.Farm.JobScripts
{
    public class JobManager : Singleton<JobManager>
    {
        public void EnqueueJob(FieldJob j, Vehicle v)
        {
            if (v == null || !v.CanDo(j.Type)) return;
            if (!j.Field.Needs(j.Type)) return;

            v.JobQueue.Clear();
            v.JobQueue.Enqueue(j);

            // --- inline DispatchIfIdle ---
            if (!v.Stats.IsBusy && v.JobQueue.TryDequeue(out var job))
            {
                GameEvents.JobStarted(v, job);
                v.StartTask(job);
            }
        }
    }
}
