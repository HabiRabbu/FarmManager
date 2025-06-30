using UnityEngine;
using Harvey.Farm.Fields;
using System.Collections;
using System.Collections.Generic;
using Harvey.Farm.Events;
using Harvey.Farm.JobScripts;
using DG.Tweening;

namespace Harvey.Farm.VehicleScripts
{
    public abstract class Vehicle : MonoBehaviour
    {
        public FieldController CurrentField { get; protected set; }
        public Queue<FieldJob> JobQueue { get; } = new();

        public abstract bool CanDo(JobType type);
        public abstract void StartTask(FieldJob job);

        // Cache
        public VehicleStats Stats { get; private set; }

        public bool IsBusy => Stats.IsBusy;
        public string DisplayName => Stats.vehicleName;

        protected virtual void Awake()
        {
            Stats = GetComponent<VehicleStats>();
        }

        protected virtual void Start()
        {
            VehicleManager.Instance.RegisterVehicle(this);
        }

        public void SetBusy(bool value)
        {
            Stats.SetBusy(value);
            GameEvents.VehicleBusyChanged(this, value);
        }

    }
}
