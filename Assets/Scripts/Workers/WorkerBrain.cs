using Harvey.Farm.Events;
using Harvey.Farm.Jobs;
using Harvey.Farm.TimeManagement;
using Harvey.Farm.Workers;

using UnityEngine;

[RequireComponent(typeof(Worker))]
public class WorkerBrain : MonoBehaviour
{
    Worker w;
    public enum State { OffShift, Looking, Working, Idle, GoingHome, WaitingForResources }
    [SerializeField] State state = State.OffShift;
    JobEntry current;
    JobExecutor exec;

    [SerializeField] float idleTime = 0f;
    const float INITIAL_JOB_SEARCH_TIME = 2f;

    public State GetCurrentState() => state;

    public JobEntry CurrentJobEntry => current;

    public void SetCurrentJob(JobEntry entry, int resumeToken)
    {
        current = entry;
        if (entry == null) return;

        w.StopAllCoroutines();
        w.Mover.StopAllTweens();
        exec.StartJob(entry.Job, resumeToken);
        state = State.Working;
        idleTime = 0f;
    }

    void Awake()
    {
        w = GetComponent<Worker>();
        exec = GetComponent<JobExecutor>();
    }

    #region Event Listeners
    void OnEnable()
    {
        SetupEventListeners(true);
    }
    void OnDisable()
    {
        SetupEventListeners(false);
        ResetState();
    }
    void OnDestroy()
    {
        SetupEventListeners(false);
    }

    void ResetState()
    {
        state = State.OffShift;
        current = null;
        idleTime = 0f;
    }

    void SetupEventListeners(bool enable)
    {
        if (enable)
        {
            GameEvents.OnShiftStarted += HandleShiftStart;
            GameEvents.OnShiftEnded += HandleShiftEnd;
            GameEvents.OnJobPosted += HandleJobPosted;
            GameEvents.OnResourcesAvailable += HandleResourcesAvailable;
        }
        else
        {
            GameEvents.OnShiftStarted -= HandleShiftStart;
            GameEvents.OnShiftEnded -= HandleShiftEnd;
            GameEvents.OnJobPosted -= HandleJobPosted;
            GameEvents.OnResourcesAvailable -= HandleResourcesAvailable;
        }
    }
    #endregion

    void HandleShiftStart()
    {
        if (state == State.OffShift)
        {
            state = State.Looking;
            idleTime = 0f;
        }
    }
    void HandleShiftEnd()
    {
        AbortJob();
    }

    void HandleJobPosted(JobEntry _)
    {
        if (state == State.Looking || state == State.Idle)
        {
            TryGetJob();
        }
    }

    void HandleResourcesAvailable()
    {
        // When resources become available, retry if waiting
        if (state == State.WaitingForResources && current != null)
        {
            Debug.Log($"[{w.DisplayName}] Resources available, resuming job...");
            ResumeWaitingJob();
        }
    }

    void Update()
    {
        switch (state)
        {
            case State.Looking:
                idleTime += Time.deltaTime;
                TryGetJob();
                if (idleTime >= INITIAL_JOB_SEARCH_TIME)
                {
                    w.Mover.StopAllTweens();
                    state = State.Idle;
                }
                break;
            case State.Idle:
                idleTime += Time.deltaTime;
                break;
            case State.Working:
                CheckJobState();
                break;
            case State.WaitingForResources:
                break;
            case State.OffShift:
                // Check shift time first - if it's work time, start looking regardless of position
                if (IsShiftTime())
                {
                    state = State.Looking;
                    idleTime = 0f;
                }
                else if (!AtHome())
                {
                    state = State.GoingHome;
                    w.ReturnHome();
                }
                break;
            case State.GoingHome:
                if (AtHome())
                {
                    state = State.OffShift;
                    idleTime = 0f;
                }
                break;
        }
    }

    void CheckJobState()
    {
        if (current == null || current.Job == null)
        {
            FinishJob();
            return;
        }

        switch (current.Job.State)
        {
            case JobState.Completed:
                FinishJob();
                break;

            case JobState.WaitingForResources:
                Debug.Log($"[{w.DisplayName}] Job is waiting for resources...");
                state = State.WaitingForResources;
                break;

            case JobState.Failed:
                Debug.LogWarning($"[{w.DisplayName}] Job failed permanently.");
                FinishJob();
                break;

            case JobState.Active:
                // Job still running
                break;
        }
    }

    void ResumeWaitingJob()
    {
        if (current?.Job == null) return;

        state = State.Working;
        exec.ResumeJob();
    }

    void TryGetJob()
    {
        JobEntry entry;

        // If has driving license - Check for vehicle operator jobs first
        if (w.Stats.Model.HasDrivingLicense)
        {
            if (JobBoard.Instance.TryTake(w.GetId(), AgentType.VehicleOperator, out entry))
            {
                StartJobEntry(entry);
                return;
            }
        }

        if (JobBoard.Instance.TryTake(w.GetId(), AgentType.FieldWorker, out entry))
        {
            StartJobEntry(entry);
            return;
        }
    }

    void StartJobEntry(JobEntry entry)
    {
        current = entry;
        int resumeToken = entry.Job.State == JobState.Paused ? entry.Job.GetResumeData() : 0;
        exec.StartJob(entry.Job, resumeToken);
        state = State.Working;
        idleTime = 0f;
    }

    void FinishJob()
    {
        state = State.Looking;
        current = null;
        idleTime = 0f;
    }
    void AbortJob()
    {
        var unfinished = exec.Abort();
        if (unfinished != null && unfinished.State != JobState.Completed)
        {
            JobBoard.Instance.Post(new JobEntry(unfinished));
        }

        state = State.OffShift;
    }

    /*──────── helpers ────────*/
    bool AtHome() =>
        Vector3.Distance(w.transform.position, w.HomePosition) < 0.2f;

    bool IsShiftTime() =>
        TimeManager.Instance.IsShiftTime(w.GetId());

}
