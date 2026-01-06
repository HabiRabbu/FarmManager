using System;
using System.Collections;

using Harvey.Farm.Jobs;
using Harvey.Farm.Movement;
using Harvey.Farm.VehicleScripts;
using Harvey.Farm.Workers;

using UnityEngine;

namespace Harvey.Farm.Jobs.VehicleField
{
    /// <summary>
    /// Step for moving a worker to the location of a vehicle before mounting.
    /// </summary>
    public class MoveWorkerToVehicleStep : IJobStep
    {
        readonly Worker _worker;
        readonly Func<Vehicle> _vehicleGetter;
        Coroutine _runningCoroutine;

        bool _movementComplete;
        bool _movementFailed;

        // Stuck detection
        const float StuckCheckInterval = 1f;
        const float StuckThreshold = 0.1f;

        float _stuckCheckTimer;
        Vector3 _lastPosition;
        Vector3 _targetPosition;

        public MoveWorkerToVehicleStep(Worker worker, Func<Vehicle> getVehicle)
        {
            _worker = worker ?? throw new ArgumentNullException(nameof(worker));
            _vehicleGetter = getVehicle ?? throw new ArgumentNullException(nameof(getVehicle));
        }

        public StepResult Tick(float dt)
        {
            if (_movementFailed)
                return StepResult.Failed;

            if (_movementComplete)
                return StepResult.Done;

            var vehicle = _vehicleGetter();
            if (vehicle == null) return StepResult.Running;

            // Start movement if not started
            if (_runningCoroutine == null)
            {
                _targetPosition = vehicle.transform.position;
                _lastPosition = _worker.transform.position;
                var mover = _worker.GetComponent<WorkerMover>();
                _runningCoroutine = _worker.StartCoroutine(MoveAndMarkComplete(mover, _targetPosition));
                return StepResult.Running;
            }

            _stuckCheckTimer += dt;

            // Check if stuck
            if (_stuckCheckTimer >= StuckCheckInterval)
            {
                var currentPos = _worker.transform.position;
                float distanceMoved = Vector3.Distance(currentPos, _lastPosition);
                float distanceToTarget = Vector3.Distance(currentPos, _targetPosition);

                if (distanceMoved < StuckThreshold && distanceToTarget > 0.5f)
                {
                    Debug.LogWarning($"MoveWorkerToVehicleStep: Worker stuck at {currentPos}");
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
                _worker.StopCoroutine(_runningCoroutine);
                _runningCoroutine = null;
            }
            _worker.GetComponent<WorkerMover>()?.StopAllTweens();
        }

        IEnumerator MoveAndMarkComplete(WorkerMover mover, Vector3 targetPosition)
        {
            yield return mover.MoveTo(targetPosition);
            _movementComplete = true;
        }
    }
}
