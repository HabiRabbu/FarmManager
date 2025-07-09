using UnityEngine;
using Harvey.Farm.Fields;
using System.Collections;
using System.Collections.Generic;
using Harvey.Farm.Events;
using Harvey.Farm.Jobs;
using DG.Tweening;
using Harvey.Farm.Buildings;

namespace Harvey.Farm.VehicleScripts
{
    public abstract class Vehicle : MonoBehaviour, IJobAgent
    {
        public FieldController CurrentField { get; protected set; }
        public Queue<FieldJob> JobQueue { get; } = new();

        public abstract bool CanDo(JobType type);
        public abstract void StartTask(FieldJob job);

        // Cache
        public VehicleStats _stats { get; private set; }

        public bool IsBusy => _stats.IsBusy;
        public string Id => _stats.Id;
        public string DisplayName => _stats.DisplayName;

        public GarageBuilding Home => _stats.GetHome();
        public void SetHome(GarageBuilding home) => _stats.SetHome(home);

        protected virtual void Awake()
        {
            _stats = GetComponent<VehicleStats>();
        }

        protected virtual void Start()
        {
            VehicleManager.Instance.RegisterVehicle(this);
        }

        public void SetBusy(bool value)
        {
            _stats.SetBusy(value);
            GameEvents.VehicleBusyChanged(this, value);
        }

        public void Enqueue(FieldJob job) => JobQueue.Enqueue(job);

        public void AttachTo(Transform anchor)
        {
            transform.SetParent(anchor, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        public void Detach() => transform.SetParent(null, true);

        public void ReturnHome()
        {
            Home.ReturnVehicle(this);
        }

    }
}
