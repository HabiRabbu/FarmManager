namespace Harvey.Farm.Jobs
{
    /// <summary>One tiny slice of work (e.g. move or act).</summary>
    public interface IJobStep
    {
        /// <returns>true when the step is fully complete</returns>
        bool Tick(float dt);

        /// <summary>Cancel any running async operations (coroutines, tweens, etc.).</summary>
        void Cancel() { } // Default empty implementation for backwards compatibility
    }
}
