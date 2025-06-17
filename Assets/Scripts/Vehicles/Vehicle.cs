using UnityEngine;
using Harvey.Farm.FieldScripts;
using System.Collections;
using System.Collections.Generic;
using Harvey.Farm.Events;
using Harvey.Farm.JobScripts;
using DG.Tweening;

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

        public void TeleportTo(Vector3 worldPos)
        {
            if (rootTransform != null)
                rootTransform.position = worldPos;
            else
                Debug.LogWarning($"{vehicleName} has no rootToMove set!");
        }

        protected IEnumerator MoveAlong(List<Vector3> waypoints,
                                System.Action<int> onArrive)
        {
            for (int i = 0; i < waypoints.Count; i++)
            {
                Vector3 target = waypoints[i];
                float dist = Vector3.Distance(rootTransform.position, target);
                float travelTime = dist / moveSpeed;

                // Build a one-off sequence for this leg
                Sequence leg = DOTween.Sequence()
                    // Turn for the first 30 % of the leg, but overlap with move
                    .Join(YawLookAt(target, travelTime * 0.30f))
                    .Join(MoveTo(target, travelTime));

                yield return leg.WaitForCompletion();
                onArrive?.Invoke(i);
            }
        }

        protected Tween YawLookAt(Vector3 target, float turnTime)
        {
            Vector3 flat = new(target.x, rootTransform.position.y, target.z);
            return rootTransform.DOLookAt(flat, turnTime, AxisConstraint.Y)
                                .SetEase(Ease.Linear);
        }

        protected Tween MoveTo(Vector3 target, float time)
        {
            return rootTransform.DOMove(target, time)
                                .SetEase(Ease.Linear);
        }

    }
}
