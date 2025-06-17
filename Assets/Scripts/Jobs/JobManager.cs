using System.Collections.Generic;
using UnityEngine;
using Harvey.Farm.Events;
using Harvey.Farm.FieldScripts;
using Harvey.Farm.VehicleScripts;

namespace Harvey.Farm.JobScripts
{
    public class JobManager : MonoBehaviour
    {
        public static JobManager Instance { get; private set; }
        void Awake() { if (Instance) Destroy(gameObject); else Instance = this; }

        public void EnqueueJob(Field field, Vehicle v, JobType type)
        {
            if (v == null || !v.CanDo(type)) return;
            if (!field.Needs(type)) return;

            v.JobQueue.Clear();
            v.JobQueue.Enqueue(new FieldJob(field, type));

            // --- inline DispatchIfIdle ---
            if (!v.IsBusy && v.JobQueue.TryDequeue(out var job))
            {
                GameEvents.JobStarted(v, job.Field, job.Type);
                v.StartTask(job);
            }
        }
    }
}
