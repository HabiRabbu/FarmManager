namespace Harvey.Farm.Jobs
{
    public sealed class JobEntry
    {
        public IJob Job { get; }
        public string OwnerId { get; }
        public AgentType RequiredAgent { get; }

        public JobEntry(IJob job)
        {
            Job = job;
            OwnerId = job.OwnerId;
            RequiredAgent = job.RequiredAgent;
        }

        public override string ToString() =>
            $"{Job.GetType().Name} ({RequiredAgent}, Owner: {OwnerId ?? "Open"})";
    }
}