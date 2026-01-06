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
        public Queue<FieldJob> JobQueue { get; } = new();
        public FieldController CurrentField => _stats.CurrentField;

        public string GetId() => _stats.GetId();
        public int CurrentTileIndex => _stats.CurrentTileIndex;

        public abstract bool CanDo(JobType type);
        public abstract void StartTask(FieldJob job, int resumeTile = 0);

        // Cache
        public VehicleStats _stats { get; private set; }

        public bool IsBusy => _stats.Model.IsBusy;
        public void SetBusy(bool value)
        {
            _stats.SetBusy(value);
            GameEvents.VehicleBusyChanged(this, value);

            // Notify waiting workers that a resource is now available
            if (!value)
                GameEvents.ResourcesAvailable();
        }

        public string DisplayName => _stats.Model.DisplayName;

        public GarageBuilding Home => _stats.GetHome();
        public void SetHome(GarageBuilding home) => _stats.SetHome(home);

        protected virtual void Awake()
        {
            _stats = GetComponent<VehicleStats>();
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

        /// <summary>
        /// Vehicles are operated by workers through VehicleFieldJobInstance steps.
        /// Direct IJob execution is not supported - workers mount and control vehicles.
        /// </summary>
        public void StartTask(IJob job, int resumeTile = 0)
        {
            Debug.LogWarning($"Vehicle {DisplayName}: StartTask(IJob) called directly. " +
                           "Vehicles should be operated via VehicleFieldJobInstance steps by workers.");
            // Behicles DON'T execute IJob directly
        }
    }
}
