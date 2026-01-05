using UnityEngine;

namespace Harvey.Farm.Jobs
{
    public interface IJob
    {
        JobState   State          { get; }
        string     OwnerId        { get; }
        AgentType  RequiredAgent  { get; }

        void Begin(IJobAgent agent, int resumeData = 0);
        void Tick(float dt);
        void Pause();
        int  GetResumeData();
    }

    public enum JobState { Pending, Active, Paused, Completed }
}
