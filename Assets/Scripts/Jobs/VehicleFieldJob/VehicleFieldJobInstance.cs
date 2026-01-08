using System.Collections.Generic;

using Harvey.Farm.Buildings;
using Harvey.Farm.Jobs;
using Harvey.Farm.Movement;
using Harvey.Farm.VehicleScripts;
using Harvey.Farm.Workers;

using UnityEngine;

namespace Harvey.Farm.Jobs.VehicleField
{
    /// <summary>Runtime vehicle-based field job that coordinates vehicle operations through multiple steps.</summary>
    public class VehicleFieldJobInstance : IJob
    {
        readonly VehicleFieldJob _def;

        const int TokenBitWidth = 16;
        const float PostJobDelay = 0.1f;

        public VehicleFieldJob Definition => _def;
        public Vehicle Vehicle { get; internal set; }
        public GarageBuilding Garage { get; internal set; }

        int _macroIdx;          // current step in the macro
        int _serpTileIdx;       // progress inside Drive step
        Queue<IJobStep> _steps;

        public string OwnerId { get; }
        public AgentType RequiredAgent => AgentType.VehicleOperator;
        public JobState State { get; private set; } = JobState.Pending;

        public VehicleFieldJobInstance(VehicleFieldJob definition, string ownerId)
            => (_def, OwnerId) = (definition, ownerId);

        /*──────── IJob ────────*/
        public void Begin(IJobAgent agent, int resumeToken = 0)
        {
            _macroIdx = resumeToken >> TokenBitWidth;
            _serpTileIdx = resumeToken & 0xFFFF;

            BuildSteps(agent as Worker);
            State = JobState.Active;

            _def.Field.BeginJob(_def.Type, _def.Crop);
        }

        public void Tick(float dt)
        {
            if (State != JobState.Active) return;

            if (_steps.Count == 0)
            {
                State = JobState.Completed;
                return;
            }

            var result = _steps.Peek().Tick(dt);
            switch (result)
            {
                case StepResult.Done:
                    _steps.Dequeue();
                    _macroIdx++;
                    break;

                case StepResult.Running:
                    // Continue ticking
                    break;

                case StepResult.WaitForResources:
                    // Signal to WorkerBrain that we need to wait
                    State = JobState.WaitingForResources;
                    return;

                case StepResult.Failed:
                    Debug.LogError($"Job step failed permanently. Aborting job.");
                    State = JobState.Failed;
                    return;
            }

            if (_steps.Count == 0)
            {
                State = JobState.Completed;
            }
        }

        public void Pause()
        {
            State = JobState.Paused;
            if (Vehicle != null)
            {
                // 1. mark tractor as not reserved
                Vehicle.SetBusy(false);

                // 2. STOP any ongoing MoveAlong / ReturnToHome coroutine
                var mover = Vehicle.GetComponent<TractorMover>();
                if (mover != null)
                {
                    mover.StopAllTweens();
                    mover.StopAllCoroutines();
                }
            }

            if (_steps != null)
            {
                foreach (var step in _steps)
                    step.Cancel();
                _steps.Clear();
            }
        }

        public void Resume()
        {
            if (State == JobState.WaitingForResources)
                State = JobState.Active;
        }

        public int GetResumeData() => (_macroIdx << TokenBitWidth) | _serpTileIdx;

        /*──────────────── helpers ────────────────*/
        void BuildSteps(Worker w)
        {
            _steps = new Queue<IJobStep>();

            bool isResumingMidDrive = _macroIdx >= 4 && _serpTileIdx > 0;
            bool vehicleAlreadyAssigned = Vehicle != null;

            if (!vehicleAlreadyAssigned && (_macroIdx <= 0 || isResumingMidDrive))
                _steps.Enqueue(new ReserveVehicleStep(this, _def.VehicleId, w));
            if (!vehicleAlreadyAssigned && (_macroIdx <= 1 || isResumingMidDrive))
                _steps.Enqueue(new MoveWorkerToVehicleStep(w, () => Vehicle));
            if (!vehicleAlreadyAssigned && (_macroIdx <= 2 || isResumingMidDrive))
                _steps.Enqueue(new MountVehicleStep(w, () => Vehicle));
            if (_macroIdx <= 3)
                _steps.Enqueue(new EnsureImplementStep(() => Vehicle, _def.ImplementId, _def.Type));
            if (_macroIdx <= 4)
                _steps.Enqueue(new DriveSerpentineStep(_def.Field,
                                                        _def.Type,
                                                        _def.Crop,
                                                      () => Vehicle,
                                                      _serpTileIdx,
                                                      i => _serpTileIdx = i));
            if (_macroIdx <= 5)
                _steps.Enqueue(new ReturnImplementStep(() => Vehicle, _def.ImplementId));
            if (_macroIdx <= 6)
                _steps.Enqueue(new ReturnVehicleStep(() => Vehicle, () => Garage));
            if (_macroIdx <= 7)
                _steps.Enqueue(new DismountVehicleStep(w, () => Vehicle));
            if (_macroIdx <= 8)
                _steps.Enqueue(new WaitStep(PostJobDelay));
        }
    }
}
