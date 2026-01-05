using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Harvey.Farm.VehicleScripts;
using DG.Tweening;
using Harvey.Farm.Events;
using UnityEngine.Rendering.Universal;

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

        void OnEnable()
        {
            GameEvents.OnStopAllTweens += StopAllTweens;
        }
        void OnDisable()
        {
            GameEvents.OnStopAllTweens -= StopAllTweens;
        }
        void OnDestroy()
        {
            GameEvents.OnStopAllTweens -= StopAllTweens;
        }

        public override void StopAllTweens()
        {
            if (seq != null && seq.IsActive())
                base.StopAllTweens();
        }

        public override IEnumerator MoveAlong(List<Vector3> wps, System.Action<int> onArrive, int resumeTile = 0)
        {
            for (int currentIndex = resumeTile; currentIndex < wps.Count; currentIndex++)
            {
                stats.SetCurrentTileIndex(currentIndex);
                float dist = Vector3.Distance(rootTransform.position, wps[currentIndex]);
                float time = dist / stats.Model.MoveSpeed;

                seq = DOTween.Sequence()
                    .Join(YawLookAt(wps[currentIndex], time * 0.3f))
                    .Join(TranslateTo(wps[currentIndex], time));

                yield return seq.WaitForCompletion();
                onArrive?.Invoke(currentIndex);
            }
        }

        public override IEnumerator ReturnToHome()
        {
            var homePos = stats.GetHome().transform.position;

            float dist = Vector3.Distance(rootTransform.position, homePos);
            float time = dist / stats.Model.MoveSpeed;

            seq = DOTween.Sequence()
                .Join(YawLookAt(homePos, time * 0.3f))
                .Join(TranslateTo(homePos, time));

            yield return seq.WaitForCompletion();
        }


    }
}
