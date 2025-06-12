using UnityEngine;
using Harvey.Farm.Events;

namespace Harvey.Farm.Utilities
{
    public class DebugManager : MonoBehaviour
    {
        private FarmInput input;
        public static bool DebugLabelsOn { get; private set; } = false;

        public static DebugManager Instance { get; private set; }

        void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        public void ToggleDebug()
        {
            DebugLabelsOn = !DebugLabelsOn;
            DebugEvents.DebugModeToggled(DebugLabelsOn);
        }
    }
}