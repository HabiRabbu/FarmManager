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

        [SerializeField] private VehicleDefinition definition;
        public VehicleDefinition Def => definition;
        public VehicleType Type => definition.Type;

        [SerializeField] private Transform rootTransform;

        [SerializeField, Tooltip("Used only when no VehicleDefinition is assigned")]
        private float fallbackMoveSpeed = 2f;
        protected float MoveSpeed => definition ? definition.MoveSpeed : fallbackMoveSpeed;

        [SerializeField] public string vehicleName = "Vehicle";


        public bool IsBusy { get; protected set; }
        public Field CurrentField { get; protected set; }

        public Queue<FieldJob> JobQueue { get; } = new();

        public abstract bool CanDo(JobType type);
        public abstract void StartTask(FieldJob job);

        void Awake()
        {
            if (!definition)
            {
                Debug.LogWarning($"<color=yellow>{name}</color> has no VehicleDefinition assigned; " +
                                 $"using fallback stats.", this);
            }
            else
            {
                vehicleName = definition.DisplayName;
                //Set others properties from definition?
            }
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

        // ------------------------------ MOVEMENT ------------------------------
        protected IEnumerator MoveAlong(List<Vector3> waypoints,
                                System.Action<int> onArrive)
        {
            for (int i = 0; i < waypoints.Count; i++)
            {
                Vector3 target = waypoints[i];
                float dist = Vector3.Distance(rootTransform.position, target);
                float travelTime = dist / MoveSpeed;

                Sequence leg = DOTween.Sequence()
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
        // ------------------------------------------------------------------------

        protected void SetBusy(bool value)
        {
            IsBusy = value;
            GameEvents.VehicleBusyChanged(this, value);
        }

    }
}
