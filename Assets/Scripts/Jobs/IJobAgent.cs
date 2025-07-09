using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Harvey.Farm.Jobs
{
    public interface IJobAgent
    {
        string DisplayName { get; }

        bool IsBusy { get; }
        bool CanDo(JobType t);

        Queue<FieldJob> JobQueue { get; }

        void Enqueue(FieldJob job);
        void StartTask(FieldJob job);

        Transform transform { get; }

        void ReturnHome();
    }
}
