using System;
using System.Collections.Generic;
using System.Linq;

using Harvey.Farm.Events;
using Harvey.Farm.Jobs;

using UnityEngine;

public class JobBoard : Singleton<JobBoard>
{
    [SerializeField, TextArea(3, 10)]
    private string jobsDisplay = "";

    readonly Dictionary<string, Queue<JobEntry>> ownerJobs = new();

    readonly Dictionary<AgentType, Queue<JobEntry>> openJobs = new();

    public void Post(JobEntry j)
    {
        if (!string.IsNullOrEmpty(j.OwnerId))
        {
            // Owner-specific job
            if (!ownerJobs.TryGetValue(j.OwnerId, out var queue))
            {
                queue = new Queue<JobEntry>();
                ownerJobs[j.OwnerId] = queue;
            }
            queue.Enqueue(j);
        }
        else
        {
            // Open job for any agent of this type
            if (!openJobs.TryGetValue(j.RequiredAgent, out var queue))
            {
                queue = new Queue<JobEntry>();
                openJobs[j.RequiredAgent] = queue;
            }
            queue.Enqueue(j);
        }

        UpdateJobsDisplay();
        GameEvents.JobPosted(j);
    }

    public bool TryTake(string agentId, AgentType type, out JobEntry entry)
    {
        // Pass 1: Check for owner-specific jobs (O(1))
        if (ownerJobs.TryGetValue(agentId, out var ownerQueue) && ownerQueue.Count > 0)
        {
            entry = ownerQueue.Dequeue();
            if (ownerQueue.Count == 0)
                ownerJobs.Remove(agentId);
            UpdateJobsDisplay();
            return true;
        }

        // Pass 2: Check for open jobs for this agent type (O(1))
        if (openJobs.TryGetValue(type, out var openQueue) && openQueue.Count > 0)
        {
            entry = openQueue.Dequeue();
            if (openQueue.Count == 0)
                openJobs.Remove(type);
            UpdateJobsDisplay();
            return true;
        }

        entry = null;
        return false;
    }

    private void UpdateJobsDisplay()
    {
        var allJobs = new List<string>();

        foreach (var kvp in ownerJobs)
            foreach (var job in kvp.Value)
                allJobs.Add($"{job} (Owner: {kvp.Key})");

        foreach (var kvp in openJobs)
            foreach (var job in kvp.Value)
                allJobs.Add($"{job} (Open: {kvp.Key})");

        jobsDisplay = allJobs.Count == 0
            ? "No jobs available"
            : string.Join("\n", allJobs.Select((s, i) => $"{i + 1}. {s}"));
    }

    private void OnValidate()
    {
        UpdateJobsDisplay();
    }

    /* ─────────── save/load support ─────────── */

    public IEnumerable<(JobEntry entry, bool isOwnerJob, string ownerId, AgentType agentType)> GetAllPendingJobs()
    {
        foreach (var kvp in ownerJobs)
            foreach (var entry in kvp.Value)
                yield return (entry, true, kvp.Key, entry.RequiredAgent);

        foreach (var kvp in openJobs)
            foreach (var entry in kvp.Value)
                yield return (entry, false, null, kvp.Key);
    }

    public void ClearAllJobs()
    {
        ownerJobs.Clear();
        openJobs.Clear();
        UpdateJobsDisplay();
    }

    public int PendingJobCount => ownerJobs.Values.Sum(q => q.Count) + openJobs.Values.Sum(q => q.Count);
}