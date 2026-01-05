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

        private bool _movementComplete = false;

        public MoveWorkerToVehicleStep(Worker worker, Func<Vehicle> getVehicle)
        {
            _worker = worker ?? throw new ArgumentNullException(nameof(worker));
            _vehicleGetter = getVehicle ?? throw new ArgumentNullException(nameof(getVehicle));
        }

        public bool Tick(float dt)
        {
            var vehicle = _vehicleGetter();
            if (vehicle == null) return false;

            if (_runningCoroutine == null && !_movementComplete)
            {
                var mover = _worker.GetComponent<WorkerMover>();
                _runningCoroutine = _worker.StartCoroutine(MoveAndMarkComplete(mover, vehicle.transform.position));
            }

            return _movementComplete;
        }

        private IEnumerator MoveAndMarkComplete(WorkerMover mover, Vector3 targetPosition)
        {
            yield return mover.MoveTo(targetPosition);
            _movementComplete = true;
        }
    }
}
