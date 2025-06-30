using UnityEngine;
using Harvey.Farm.Events;

namespace Harvey.Farm.Utilities
{
    public class DebugManager : Singleton<DebugManager>
    {
        private FarmInput input;
        public static bool DebugLabelsOn { get; private set; } = false;

        public void ToggleDebug()
        {
            DebugLabelsOn = !DebugLabelsOn;
            GameEvents.DebugModeToggled(DebugLabelsOn);
        }
    }
}