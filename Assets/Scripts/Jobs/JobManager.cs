using System.Collections.Generic;
using Harvey.Farm.Events;
using Harvey.Farm.FieldScripts;
using Harvey.Farm.VehicleScripts;
using UnityEngine;

namespace Harvey.Farm.JobScripts
{
    public class JobManager : MonoBehaviour
    {
        public static JobManager Instance { get; private set; }
        void Awake() { if (Instance) Destroy(gameObject); else Instance = this; }

        public void EnqueuePlowJob(Field field, Vehicle tractor)
        {
            if (tractor == null) return;
            field.EnqueuePlowJob(tractor);
        }

        void DispatchIfIdle(Vehicle v)
        {
            if (!v.IsBusy && v.JobQueue.Count > 0)
                v.StartTask(v.JobQueue.Dequeue());
        }

        void OnEnable()
        {
            VehicleEvents.OnVehicleBusyChanged += (v, busy) =>
            {
                if (!busy && v.CurrentField != null)
                    v.CurrentField.DispatchNextTile(v);
            };
        }
    }
}

