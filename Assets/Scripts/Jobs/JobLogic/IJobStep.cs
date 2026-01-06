namespace Harvey.Farm.Jobs
{
    /// <summary>Result of a step's Tick operation.</summary>
    public enum StepResult
    {
        /// <summary>Step is still running, continue ticking.</summary>
        Running,

        /// <summary>Step completed successfully, proceed to next step.</summary>
        Done,

        /// <summary>Resource temporarily unavailable (e.g. vehicle in use). Worker should wait and retry.</summary>
        WaitForResources,

        /// <summary>Step failed permanently (e.g. resource doesn't exist). Job should be aborted.</summary>
        Failed
    }

    /// <summary>One tiny slice of work (e.g. move or act).</summary>
    public interface IJobStep
    {
        /// <summary>Tick the step. Returns the result of this tick.</summary>
        StepResult Tick(float dt);

        /// <summary>Cancel any running async operations (coroutines, tweens, etc.).</summary>
        void Cancel() { } // Default empty implementation for backwards compatibility
    }
}
