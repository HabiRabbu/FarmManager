using UnityEngine;

using System.Collections;

using Harvey.Farm.Movement;

namespace Harvey.Farm.Jobs
{
    public class MoveToStep : IJobStep
    {
        readonly IMover mover;
        readonly Vector3 target;
        Coroutine _runningCoroutine;
        bool _movementCompleted;
        bool _movementFailed;

        // Stuck detection
        const float StuckCheckInterval = 1f;
        const float StuckThreshold = 0.1f;

        float _stuckCheckTimer;
        Vector3 _lastPosition;

        public MoveToStep(IMover mover, Vector3 target) =>
            (this.mover, this.target) = (mover, target);

        public StepResult Tick(float dt)
        {
            // Already failed
            if (_movementFailed)
                return StepResult.Failed;

            // Already done
            if (_movementCompleted)
                return StepResult.Done;

            // Start movement if not started
            if (_runningCoroutine == null)
            {
                var mono = (MonoBehaviour)mover;
                _lastPosition = mono.transform.position;
                _runningCoroutine = mono.StartCoroutine(MoveToWithCallback());
                return StepResult.Running;
            }

            _stuckCheckTimer += dt;

            // Check if stuck (not making progress)
            if (_stuckCheckTimer >= StuckCheckInterval)
            {
                var currentPos = ((MonoBehaviour)mover).transform.position;
                float distanceMoved = Vector3.Distance(currentPos, _lastPosition);
                float distanceToTarget = Vector3.Distance(currentPos, target);

                // If we haven't moved much AND we're not close to the target
                if (distanceMoved < StuckThreshold && distanceToTarget > 0.5f)
                {
                    Debug.LogWarning($"MoveToStep: Worker appears stuck at {currentPos}, target was {target}");
                    _movementFailed = true;
                    Cancel();
                    return StepResult.Failed;
                }

                _lastPosition = currentPos;
                _stuckCheckTimer = 0f;
            }

            return StepResult.Running;
        }

        public void Cancel()
        {
            if (_runningCoroutine != null)
            {
                ((MonoBehaviour)mover).StopCoroutine(_runningCoroutine);
                _runningCoroutine = null;
            }
            mover.StopAllTweens();
        }

        IEnumerator MoveToWithCallback()
        {
            yield return mover.MoveTo(target);
            _movementCompleted = true;
        }
    }
}
