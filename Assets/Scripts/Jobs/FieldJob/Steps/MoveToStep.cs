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
        bool _movementCompleted = false;

        public MoveToStep(IMover mover, Vector3 target) =>
            (this.mover, this.target) = (mover, target);

        public bool Tick(float dt)
        {
            if (_runningCoroutine == null && !_movementCompleted)
            {
                _runningCoroutine = ((MonoBehaviour)mover).StartCoroutine(MoveToWithCallback());
                return false; // Movement just started, not completed yet
            }

            // Return true only when the movement has completed
            return _movementCompleted;
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
            _movementCompleted = true; // Signal completion
        }
    }
}
