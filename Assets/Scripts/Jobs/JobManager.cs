using System.Collections.Generic;
using UnityEngine;
using Harvey.Farm.Events;
using Harvey.Farm.Fields;
using Harvey.Farm.VehicleScripts;
using Harvey.Farm.Workers;
using Harvey.SaveSystem;
using Harvey.Data.Jobs;
using Harvey.Data.Coffee;
using System.Threading.Tasks;

namespace Harvey.Farm.Jobs
{
    public class JobManager : Singleton<JobManager>, ISaveSection
    {
        [SerializeField] public int LoadPriority { get; } = 100;

        public void EnqueueJob(FieldJob j, IJobAgent agent)
        {
            if (agent == null || !agent.CanDo(j.Type)) return;

            bool fieldAlreadyBusy = !j.Field.Needs(j.Type);
            bool agentIsWorker = agent is Worker;

            if (fieldAlreadyBusy && !agentIsWorker)
                return;                                   // tractors get rejected

            agent.JobQueue.Clear();
            agent.JobQueue.Enqueue(j);

            Debug.Log($"JobManager.EnqueueJob ▸ Enqueued job {j.Type} for {agent.DisplayName} on {j.Field.Model.DisplayName}");

            if (!agent.IsBusy && agent.JobQueue.TryDequeue(out var job))
            {
                GameEvents.JobStarted(agent, job);
                agent.StartTask(job);
            }
        }

        public void EnqueueExistingJob(FieldJob j, IJobAgent agent, int resumeTile = 0)
        {
            if (agent.IsBusy == true)
                agent.SetBusy(false);

            if (agent == null || !agent.CanDo(j.Type)) return;

            bool fieldAlreadyBusy = !j.Field.Needs(j.Type);
            bool agentIsWorker = agent is Worker;

            if (fieldAlreadyBusy && !agentIsWorker)
                return;                                   // tractors get rejected

            agent.JobQueue.Clear();
            agent.JobQueue.Enqueue(j);

            Debug.Log($"JobManager.EnqueueExistingJob ▸ Enqueued job {j.Type} for {agent.DisplayName} on {j.Field.Model.DisplayName}");

            if (!agent.IsBusy && agent.JobQueue.TryDequeue(out var job))
            {
                Debug.Log("The Task has been started");
                agent.StartTask(job, resumeTile);
            }
        }

        void Start()
        {
            SaveService.Instance.Register(this);
        }

        public void Capture(GameSaveData root)
        {
            if (root.Jobs == null)
                root.Jobs = new JobSection();

            root.Jobs.Active.Clear();

            foreach (var vehicle in VehicleManager.Instance.AllVehicles)
            {
                CaptureVehicleJobs(vehicle, root.Jobs);
            }
            foreach (var worker in WorkerManager.Instance.AllWorkers)
            {
                CaptureWorkerJobs(worker, root.Jobs);
            }
        }

        public Task Restore(GameSaveData root)
        {
            if (root.Jobs?.Active == null) return Task.CompletedTask;

            foreach (var jobData in root.Jobs.Active)
            {
                RestoreActiveJob(jobData);
            }

            Debug.Log($"JobManager.Restore ▸ Restored {root.Jobs.Active.Count} active jobs");
            return Task.CompletedTask;
        }

        private void CaptureVehicleJobs(IJobAgent agent, JobSection jobs)
        {
            if (!agent.IsBusy) return;

            if (agent.JobQueue.Count > 0)
            {
                var j = agent.JobQueue.Peek();
                jobs.Active.Add(new ActiveJobSaveData
                {
                    AgentId = agent.GetId(),
                    FieldId = j.Field.GetId(),
                    Type = j.Type,
                    CropId = j.Crop?.Id ?? string.Empty,
                    ToolId = j.ToolId,
                    NextTileIndex = agent.CurrentTileIndex,
                    TilesCompleted = agent.CurrentTileIndex
                });
            }
            else if (agent.CurrentField != null)
            {
                var currentJobType = agent.CurrentField.GetCurrentJobType();
                if (currentJobType.HasValue)
                {
                    jobs.Active.Add(new ActiveJobSaveData
                    {
                        AgentId = agent.GetId(),
                        FieldId = agent.CurrentField.GetId(),
                        Type = currentJobType.Value,
                        CropId = agent.CurrentField.currentCrop?.Id ?? string.Empty,
                        ToolId = GetToolIdForAgent(agent),
                        NextTileIndex = agent.CurrentTileIndex,
                        TilesCompleted = agent.CurrentTileIndex
                    });
                }
            }
        }

        private void CaptureWorkerJobs(IJobAgent agent, JobSection jobs)
        {
            if (!agent.IsBusy) return;

            if (agent.JobQueue.Count > 0)
            {
                var j = agent.JobQueue.Peek();
                jobs.Active.Add(new ActiveJobSaveData
                {
                    AgentId = agent.GetId(),
                    FieldId = j.Field?.GetId() ?? string.Empty,
                    Type = j.Type,
                    CropId = j.Crop?.Id ?? string.Empty,
                    ToolId = j.ToolId ?? string.Empty,
                    NextTileIndex = agent.CurrentTileIndex,
                    TilesCompleted = agent.CurrentField.runtime.TilesCompleted
                });
            }
            else if (agent.CurrentField != null)
            {
                var currentJobType = agent.CurrentField.GetCurrentJobType();
                if (currentJobType.HasValue)
                {
                    jobs.Active.Add(new ActiveJobSaveData
                    {
                        AgentId = agent.GetId(),
                        FieldId = agent.CurrentField.GetId(),
                        Type = currentJobType.Value,
                        CropId = agent.CurrentField.currentCrop?.Id ?? string.Empty,
                        ToolId = GetToolIdForAgent(agent) ?? string.Empty,
                        NextTileIndex = agent.CurrentTileIndex,
                        TilesCompleted = agent.CurrentField.runtime.TilesCompleted
                    });
                }
            }
        }

        private string GetToolIdForAgent(IJobAgent agent)
        {
            return agent switch
            {
                Tractor tractor => tractor.TractorModel.AttachedToolId ?? string.Empty,
                CombineHarvester => string.Empty,
                _ => string.Empty
            };
        }

        private void RestoreActiveJob(ActiveJobSaveData jobData)
        {
            var agent = GetJobAgentById(jobData.AgentId);
            if (agent == null)
            {
                Debug.LogWarning($"JobManager.Restore ▸ Job lost agent {jobData.AgentId}");
                return;
            }

            var field = FieldManager.Instance.GetById(jobData.FieldId);
            if (field == null)
            {
                Debug.LogWarning($"JobManager.Restore ▸ Job lost field {jobData.FieldId}");
                return;
            }

            CoffeeCropData crop = null;
            if (!string.IsNullOrEmpty(jobData.CropId))
            {
                crop = CoffeeManager.Instance.GetById(jobData.CropId);
                if (crop == null)
                {
                    Debug.LogWarning($"JobManager.Restore ▸ Job lost crop {jobData.CropId}");
                }
            }

            var job = new FieldJob(field, jobData.Type, crop, jobData.ToolId);

            field.BeginJob(jobData.Type, crop);
            field.runtime.SetProgress(jobData.TilesCompleted);

            EnqueueExistingJob(job, agent, jobData.NextTileIndex);

            Debug.Log($"JobManager.Restore ▸ Restored job {jobData.Type} for {agent.DisplayName} on {field.Model.DisplayName}");
        }

        private IJobAgent GetJobAgentById(string id)
        {
            var vehicle = VehicleManager.Instance.GetById(id);
            if (vehicle != null) return vehicle;

            var worker = WorkerManager.Instance.GetById(id);
            if (worker != null) return worker;

            return null;
        }
    }
}
