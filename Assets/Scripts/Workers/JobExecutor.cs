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

        void Update()
        {
            if (_current == null) return;
            _current.Tick(Time.deltaTime);

            if (_current.State == JobState.Completed)
            {
                GameEvents.JobCompleted(_current);
                _current = null;
            }
        }
    }
}
