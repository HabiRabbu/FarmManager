using System.Collections.Generic;
using UnityEngine;
using Harvey.Farm.Events;
using Harvey.Farm.Fields;
using Harvey.Farm.VehicleScripts;
using Harvey.Farm.Crops;
using Harvey.Farm.Workers;

namespace Harvey.Farm.Jobs
{
    public class JobManager : Singleton<JobManager>
    {
        public void EnqueueJob(FieldJob j, IJobAgent agent)
        {
            if (agent == null || !agent.CanDo(j.Type)) return;

            bool fieldAlreadyBusy = !j.Field.Needs(j.Type);
            bool agentIsWorker = agent is Worker;

            if (fieldAlreadyBusy && !agentIsWorker)
                return;                                   // tractors get rejected

            agent.JobQueue.Clear();
            agent.JobQueue.Enqueue(j);

            if (!agent.IsBusy && agent.JobQueue.TryDequeue(out var job))
            {
                GameEvents.JobStarted(agent, job);
                agent.StartTask(job);
            }
        }

    }
}
