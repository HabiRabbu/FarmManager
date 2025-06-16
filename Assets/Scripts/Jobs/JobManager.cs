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

        bool JobNeeded(FieldTile t, JobType type) => type switch
        {
            JobType.Plow    => !t.IsPlowed,
            JobType.Seed    => t.IsPlowed && !t.IsSeeded,
            JobType.Harvest => t.IsSeeded && !t.IsHarvested,
            _               => false
        };

        public void EnqueueJob(Field field, Vehicle v, JobType type)
        {
            if (v == null || !v.CanDo(type)) return;

            foreach (var t in field.GetSerpentineTiles())
                if (JobNeeded(t, type))
                    v.JobQueue.Enqueue(new FieldJob(t, type, field));

            DispatchIfIdle(v, field, type);

        }

        void DispatchIfIdle(Vehicle v, Field f, JobType j)
        {
            if (v.IsBusy || v.JobQueue.Count == 0) return;

            var job = v.JobQueue.Dequeue();

            GameEvents.JobStarted(v, f, j);
            v.StartTask(job);
        }
    }
}
