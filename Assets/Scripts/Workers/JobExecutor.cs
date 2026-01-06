using Harvey.Farm.Events;
using Harvey.Farm.Jobs;
using UnityEngine;

namespace Harvey.Farm.Workers
{
    [RequireComponent(typeof(Worker))]
    public class JobExecutor : MonoBehaviour
    {
        IJob _current;
        Worker _worker;

        void Awake() => _worker = GetComponent<Worker>();

        public bool IsRunning => _current != null && _current.State == JobState.Active;
        public bool IsWaitingForResources => _current != null && _current.State == JobState.WaitingForResources;

        public void StartJob(IJob job, int resumeToken = 0)
        {
            _current = job;
            _current.Begin(_worker, resumeToken);
        }

        public IJob Abort()
        {
            if (_current == null) return null;

            _current.Pause();
            GameEvents.JobPaused(_current);
            var tmp = _current;
            _current = null;
            return tmp;
        }

        /// <summary>
        /// Cancels the current job without reposting it. Used when the job should be discarded entirely.
        /// </summary>
        public void CancelCurrentJob()
        {
            if (_current == null) return;

            _current.Pause();
            GameEvents.JobPaused(_current);
            _current = null;
        }

        /// <summary>
        /// Resume a job that was waiting for resources.
        /// </summary>
        public void ResumeJob()
        {
            if (_current == null) return;

            if (_current.State == JobState.WaitingForResources)
            {
                _current.Resume();
                Debug.Log($"Resumed job from WaitingForResources state");
            }
        }

        void Update()
        {
            if (_current == null) return;

            // Only tick if the job is active
            if (_current.State == JobState.Active)
            {
                _current.Tick(Time.deltaTime);
            }

            if (_current.State == JobState.Completed)
            {
                GameEvents.JobCompleted(_current);
                _current = null;
            }
            else if (_current.State == JobState.Failed)
            {
                GameEvents.JobFailed(_current, "Job step failed permanently");
                _current = null;
            }
        }
    }
}
