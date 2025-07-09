using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Harvey.Farm.VehicleScripts;
using DG.Tweening;

namespace Harvey.Farm.Movement
{
    public class TractorMover : BaseMover
    {
        VehicleStats stats;

        protected override void Awake()
        {
            base.Awake();
            stats = GetComponent<VehicleStats>();
        }

        public override IEnumerator MoveAlong(List<Vector3> wps,
                                              System.Action<int> onArrive)
        {
            for (int i = 0; i < wps.Count; i++)
            {
                float dist = Vector3.Distance(rootTransform.position, wps[i]);
                float time = dist / stats.MoveSpeed;

                var seq = DOTween.Sequence()
                    .Join(YawLookAt(wps[i], time * 0.3f))
                    .Join(TranslateTo(wps[i], time));

                yield return seq.WaitForCompletion();
                onArrive?.Invoke(i);
            }
        }

        public override IEnumerator ReturnToHome()
        {
            var homePos = stats.GetHome().transform.position;

            float dist = Vector3.Distance(rootTransform.position, homePos);
            float time = dist / stats.MoveSpeed;

            var seq = DOTween.Sequence()
                .Join(YawLookAt(homePos, time * 0.3f))
                .Join(TranslateTo(homePos, time));

            yield return seq.WaitForCompletion();
        }
    }
}
