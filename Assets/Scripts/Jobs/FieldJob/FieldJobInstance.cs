using System.Collections.Generic;

using Harvey.Farm.Fields;
using Harvey.Farm.Movement;
using Harvey.Farm.Workers;

using UnityEngine;

namespace Harvey.Farm.Jobs
{
    /// <summary>Runtime plow/seed/harvest job that progresses tile-by-tile.</summary>
    public class FieldJobInstance : IJob
    {
        const float InterTileDelay = 0.1f;

        readonly FieldJob _def;
        readonly Queue<IJobStep> _steps = new();
        IJobAgent _agent;
        int _tileIndex;
        JobState _state = JobState.Pending;
        FieldTile _currentReservedTile;

        public FieldJob Definition => _def;
        public string OwnerId { get; }
        public AgentType RequiredAgent => AgentType.FieldWorker;
        public JobState State => _state;

        public FieldJobInstance(FieldJob definition, string ownerId)
            => (_def, OwnerId) = (definition, ownerId);

        /*──────── IJob ────────*/
        public void Begin(IJobAgent agent, int resumeToken = 0)
        {
            _agent = agent;
            _tileIndex = resumeToken;
            _state = JobState.Active;

            _def.Field.BeginJob(_def.Type, _def.Crop);
            BuildNextTileSteps();                 // prime first tile
        }

        public void Tick(float dt)
        {
            if (_state != JobState.Active) return;

            if (_steps.Count == 0)
            {
                if (!BuildNextTileSteps())        // any tiles left?
                {
                    _state = JobState.Completed;
                    return;
                }
            }

            var result = _steps.Peek().Tick(dt);
            switch (result)
            {
                case StepResult.Done:
                    _steps.Dequeue();
                    break;

                case StepResult.Running:
                    // Continue ticking
                    break;

                case StepResult.WaitForResources:
                    _state = JobState.WaitingForResources;
                    return;

                case StepResult.Failed:
                    Debug.LogError($"Job step failed permanently. Aborting job.");
                    _state = JobState.Failed;
                    return;
            }
        }

        public void Pause()
        {
            _state = JobState.Paused;

            var mover = ((MonoBehaviour)_agent).GetComponent<IMover>() as WorkerMover;
            mover?.StopAllTweens();

            _currentReservedTile?.ClearReservation();
            _currentReservedTile = null;

            foreach (var step in _steps)
                step.Cancel();
            _steps.Clear();
        }

        public void Resume()
        {
            if (_state == JobState.WaitingForResources)
                _state = JobState.Active;
        }

        public int GetResumeData() => _tileIndex;

        /*──────── helpers ────────*/
        bool BuildNextTileSteps()
        {
            FieldTile tile = _def.Field.GetNearestAvailableTile(
                                 _def.Type,
                                 ((MonoBehaviour)_agent).transform.position);
            if (tile == null) return false;

            // (1) reserve + move
            _currentReservedTile = tile;
            tile.TryReserve();
            _steps.Enqueue(new MoveToStep(((MonoBehaviour)_agent)
                            .GetComponent<IMover>(), tile.WorldPosition));

            // (2) act on tile (clears reservation when complete)
            _steps.Enqueue(new FieldActionStep(tile, _def.Type, _def.Crop, ((MonoBehaviour)_agent).GetComponent<Worker>()));

            // (3) leave a tiny idle frame so the brain can interleave work
            _steps.Enqueue(new WaitStep(InterTileDelay));

            _tileIndex++;
            return true;
        }
    }
}
