using System;

/// <summary>
/// Central event bus for all debug-related signals.  
/// Subscribers register in OnEnable, unregister in OnDisable.
/// </summary>

namespace Harvey.Farm.Events
{
    public static class DebugEvents
    {
        public static event Action<bool> OnDebugModeToggled;


        public static void DebugModeToggled(bool enabled) => OnDebugModeToggled?.Invoke(enabled);
    }
}
