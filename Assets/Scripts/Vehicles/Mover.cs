using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

namespace Harvey.Farm.VehicleScripts
{
    public class Mover : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private Transform rootTransform;

        VehicleStats _vehicleStats;

        void Awake()
        {
            _vehicleStats = GetComponent<VehicleStats>();

            if (!rootTransform) rootTransform = transform;
        }

        public IEnumerator MoveTo(Vector3 worldPosition)
            => MoveAlong(new List<Vector3> { worldPosition }, null);

        public IEnumerator MoveAlong(List<Vector3> waypoints,
                                     System.Action<int> onArrive)
        {
            for (int i = 0; i < waypoints.Count; i++)
            {
                Vector3 target = waypoints[i];
                float dist = Vector3.Distance(rootTransform.position, target);
                float travelTime = dist / _vehicleStats.moveSpeed;

                Sequence leg = DOTween.Sequence()
                    .Join(YawLookAt(target, travelTime * 0.30f))
                    .Join(TranslateTo(target, travelTime));

                yield return leg.WaitForCompletion();
                onArrive?.Invoke(i);
            }
        }

        Tween YawLookAt(Vector3 target, float turnTime)
        {
            Vector3 flat = new Vector3(target.x, rootTransform.position.y, target.z);
            return rootTransform.DOLookAt(flat, turnTime, AxisConstraint.Y)
                                .SetEase(Ease.Linear);
        }

        Tween TranslateTo(Vector3 target, float travelTime)
        {
            return rootTransform.DOMove(target, travelTime)
                                .SetEase(Ease.Linear);
        }

        public void TeleportTo(Vector3 worldPos)
        {
            if (rootTransform != null)
                rootTransform.position = worldPos;
            else
                Debug.LogWarning($"{_vehicleStats.vehicleName} has no rootToMove set!");
        }
    }
}
