using UnityEngine;
using Harvey.Farm.FieldScripts;
using System.Collections;
using System.Collections.Generic;
using Harvey.Farm.Events;

namespace Harvey.Farm.VehicleScripts
{
    public abstract class Vehicle : MonoBehaviour
    {

        [SerializeField] private Transform rootTransform;
        [SerializeField] private float moveSpeed = 2f;

        [SerializeField] public string vehicleName = "Vehicle";


        public bool IsBusy { get; protected set; }
        public Field CurrentField { get; protected set; }

        public Queue<FieldJob> JobQueue { get; } = new();

        public abstract bool CanDo(JobType type);
        public abstract void StartTask(FieldJob job);

        protected void SetBusy(bool value)
        {
            IsBusy = value;
            GameEvents.VehicleBusyChanged(this, value);
        }

        protected virtual void Start()
        {
            VehicleManager.Instance.RegisterVehicle(this);
        }

        void FlatLookAt(Vector3 target)
        {
            // Build a target point that has SAME Y as the tractor, so no pitch, just yaw..
            Vector3 flatTarget = new Vector3(target.x, rootTransform.position.y, target.z);
            rootTransform.LookAt(flatTarget);
        }

        public void TeleportTo(Vector3 worldPos)
        {
            if (rootTransform != null)
                rootTransform.position = worldPos;
            else
                Debug.LogWarning($"{vehicleName} has no rootToMove set!");
        }

        protected IEnumerator MoveAlong(List<Vector3> waypoints, System.Action<int> onArrive)
        {
            for (int i = 0; i < waypoints.Count; i++)
            {
                Vector3 target = waypoints[i];
                FlatLookAt(target);
                while (Vector3.Distance(rootTransform.position, target) > 0.01f)
                {
                    rootTransform.position = Vector3.MoveTowards(
                        rootTransform.position,
                        target,
                        moveSpeed * Time.deltaTime);
                    yield return null; // Wait af rame
                }

                onArrive?.Invoke(i);
            }
        }
    }
}
