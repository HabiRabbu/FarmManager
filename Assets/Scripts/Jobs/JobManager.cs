using System.Collections.Generic;
using UnityEngine;
using Harvey.Farm.Events;
using Harvey.Farm.Fields;
using Harvey.Farm.VehicleScripts;
using Harvey.Farm.Workers;
using Harvey.SaveSystem;
using Harvey.Data.Jobs;
using Harvey.Data.Coffee;
using Harvey.Farm.Jobs.VehicleField;
using System.Threading.Tasks;

namespace Harvey.Farm.Jobs
{
    public class JobManager : Singleton<JobManager>, ISaveSection
    {
        [SerializeField] private int loadPriority = 100;
        public int LoadPriority => loadPriority;

        void Start()
        {
            SaveService.Instance.Register(this);
        }

        public void Capture(GameSaveData root)
        {
            root.Jobs ??= new JobSection();
            root.Jobs.Active.Clear();

            // 1. Capture all pending jobs on the JobBoard
            CapturePendingJobs(root.Jobs);

            // 2. Capture all active jobs currently being executed by workers
            CaptureActiveWorkerJobs(root.Jobs);

            Debug.Log($"JobManager.Capture ▸ Saved {root.Jobs.Active.Count} jobs");
        }

        void CapturePendingJobs(JobSection jobs)
        {
            foreach (var (entry, isOwnerJob, ownerId, agentType) in JobBoard.Instance.GetAllPendingJobs())
            {
                var saveData = CreateSaveDataFromJob(entry.Job, ownerId, JobSaveState.Pending, isOwnerJob);
                if (saveData.HasValue)
                    jobs.Active.Add(saveData.Value);
            }
        }

        void CaptureActiveWorkerJobs(JobSection jobs)
        {
            foreach (var worker in WorkerManager.Instance.AllWorkers)
            {
                var brain = worker.Brain;
                if (brain == null) continue;

                var entry = brain.CurrentJobEntry;
                if (entry?.Job == null) continue;

                var state = entry.Job.State switch
                {
                    JobState.Active => JobSaveState.Active,
                    JobState.Paused => JobSaveState.Paused,
                    JobState.WaitingForResources => JobSaveState.WaitingForResources,
                    _ => JobSaveState.Active
                };

                var saveData = CreateSaveDataFromJob(entry.Job, worker.GetId(), state, true);
                if (saveData.HasValue)
                    jobs.Active.Add(saveData.Value);
            }
        }

        ActiveJobSaveData? CreateSaveDataFromJob(IJob job, string agentId, JobSaveState state, bool isOwnerJob)
        {
            switch (job)
            {
                case FieldJobInstance fieldJob:
                    return CreateFieldJobSaveData(fieldJob, agentId, state, isOwnerJob);

                case VehicleFieldJobInstance vehicleJob:
                    return CreateVehicleJobSaveData(vehicleJob, agentId, state, isOwnerJob);

                default:
                    Debug.LogWarning($"JobManager: Unknown job type {job.GetType().Name}, cannot serialize");
                    return null;
            }
        }

        ActiveJobSaveData CreateFieldJobSaveData(FieldJobInstance job, string agentId, JobSaveState state, bool isOwnerJob)
        {
            var def = job.Definition;

            return new ActiveJobSaveData
            {
                AgentId = agentId,
                FieldId = def.Field?.GetId() ?? string.Empty,
                Type = def.Type,
                CropId = def.Crop?.Id ?? string.Empty,
                ToolId = def.ToolId ?? string.Empty,
                ResumeToken = job.GetResumeData(),
                TilesCompleted = def.Field?.runtime?.TilesCompleted ?? 0,
                IsVehicleJob = false,
                VehicleId = string.Empty,
                ImplementId = string.Empty,
                RequiredAgent = AgentType.FieldWorker,
                State = state,
                IsOwnerJob = isOwnerJob
            };
        }

        ActiveJobSaveData CreateVehicleJobSaveData(VehicleFieldJobInstance job, string agentId, JobSaveState state, bool isOwnerJob)
        {
            var def = job.Definition;

            return new ActiveJobSaveData
            {
                AgentId = agentId,
                FieldId = def.Field?.GetId() ?? string.Empty,
                Type = def.Type,
                CropId = def.Crop?.Id ?? string.Empty,
                ToolId = string.Empty,
                ResumeToken = job.GetResumeData(),
                TilesCompleted = def.Field?.runtime?.TilesCompleted ?? 0,
                IsVehicleJob = true,
                VehicleId = def.VehicleId ?? string.Empty,
                ImplementId = def.ImplementId ?? string.Empty,
                RequiredAgent = AgentType.VehicleOperator,
                State = state,
                IsOwnerJob = isOwnerJob
            };
        }

        public Task Restore(GameSaveData root)
        {
            if (root.Jobs?.Active == null || root.Jobs.Active.Count == 0)
                return Task.CompletedTask;

            JobBoard.Instance.ClearAllJobs();

            int restoredCount = 0;
            foreach (var jobData in root.Jobs.Active)
            {
                if (RestoreJob(jobData))
                    restoredCount++;
            }

            Debug.Log($"JobManager.Restore ▸ Restored {restoredCount}/{root.Jobs.Active.Count} jobs");
            return Task.CompletedTask;
        }

        bool RestoreJob(ActiveJobSaveData data)
        {
            var field = FieldManager.Instance.GetById(data.FieldId);
            if (field == null)
            {
                Debug.LogWarning($"JobManager.Restore ▸ Job lost field {data.FieldId}");
                return false;
            }

            CoffeeCropData crop = string.IsNullOrEmpty(data.CropId) ? null : CoffeeManager.Instance.GetById(data.CropId);

            IJob job;
            if (data.IsVehicleJob)
            {
                var def = new VehicleFieldJob(field, data.Type, crop, data.VehicleId, data.ImplementId);
                var vehicleJob = new VehicleFieldJobInstance(def, data.IsOwnerJob ? data.AgentId : null);

                if (data.State != JobSaveState.Pending && !string.IsNullOrEmpty(data.VehicleId))
                {
                    var vehicle = VehicleManager.Instance.GetById(data.VehicleId);
                    if (vehicle != null)
                    {
                        vehicleJob.Vehicle = vehicle;
                        vehicleJob.Garage = vehicle.Home;
                        vehicle.SetBusy(true);
                    }
                }

                job = vehicleJob;
            }
            else
            {
                var def = new FieldJob(field, data.Type, crop, data.ToolId);
                job = new FieldJobInstance(def, data.IsOwnerJob ? data.AgentId : null);
            }

            switch (data.State)
            {
                case JobSaveState.Pending:
                    JobBoard.Instance.Post(new JobEntry(job));
                    break;

                case JobSaveState.Active:
                case JobSaveState.Paused:
                case JobSaveState.WaitingForResources:
                    field.BeginJob(data.Type, crop);
                    field.runtime?.SetProgress(data.TilesCompleted);

                    if (data.IsVehicleJob && job is VehicleFieldJobInstance vehicleJobInst && vehicleJobInst.Vehicle != null)
                    {
                        var worker = WorkerManager.Instance.GetById(data.AgentId);
                        if (worker != null)
                        {
                            worker.transform.SetParent(vehicleJobInst.Vehicle.transform, false);
                            worker.transform.localPosition = Vector3.zero;
                            worker.transform.localRotation = Quaternion.identity;
                            worker.Meshes.SetActive(false);
                            worker.Brain.SetCurrentJob(new JobEntry(job), data.ResumeToken);
                        }
                        else
                        {
                            JobBoard.Instance.Post(new JobEntry(job));
                        }
                    }
                    else
                    {
                        JobBoard.Instance.Post(new JobEntry(job));
                    }
                    break;
            }

            return true;
        }
    }
}
