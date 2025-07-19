using System.Collections;
using System.Collections.Generic;
using Harvey.Farm.Fields;
using UnityEngine;

namespace Harvey.Farm.Jobs
{
    public interface IJobAgent
    {
        string DisplayName { get; }
        string GetId();
        int CurrentTileIndex { get; }

        bool IsBusy { get; }
        void SetBusy(bool value);
        bool CanDo(JobType t);

        Queue<FieldJob> JobQueue { get; }
        FieldController CurrentField { get; }

        void Enqueue(FieldJob job);
        void StartTask(FieldJob job, int resumeTile = 0);

        Transform transform { get; }

        void ReturnHome();
    }
}
