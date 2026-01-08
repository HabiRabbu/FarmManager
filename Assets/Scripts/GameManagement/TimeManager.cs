using System;
using System.Threading.Tasks;

using Harvey.Farm.Events;
using Harvey.SaveSystem;

using UnityEngine;

namespace Harvey.Farm.TimeManagement
{
    public class TimeManager : Singleton<TimeManager>, ISaveSection
    {
        [Tooltip("Real-time minutes that make up one full in-game day (0-24h).")]
        [SerializeField] private float realMinutesPerDay = 10f;

        public const int SHIFT_START = 9;
        public const int SHIFT_END = 17;

        public float Hour { get; private set; }

        public float GetTime() => Hour;

        void Start()
        {
            SaveService.Instance.Register(this);
        }

        // ─────────────────── Save/Load ───────────────────
        public int LoadPriority => 0;

        public void Capture(GameSaveData root)
        {
            root.Time.Hour = Hour;
        }

        public Task Restore(GameSaveData root)
        {
            SetTimeImmediate(root.Time.Hour);
            return Task.CompletedTask;
        }

        public void SetTimeImmediate(float hour)
        {
            Hour = Mathf.Clamp(hour, 0f, 23.999f);
            _lastWholeHour = Mathf.FloorToInt(Hour);
            GameEvents.TimeChanged(_lastWholeHour);
        }

        // ────────────────────────────────────────────────
        void Update()
        {
            // advance the clock
            Hour += (24f / (realMinutesPerDay * 60f)) * Time.deltaTime;
            if (Hour >= 24f) Hour -= 24f;

            int wholeHour = Mathf.FloorToInt(Hour);

            // Fire hour changed event when we move to a new hour
            if (wholeHour != _lastWholeHour)
            {
                GameEvents.TimeChanged(wholeHour);

                if (wholeHour == SHIFT_START)
                    GameEvents.ShiftStarted();
                else if (wholeHour == SHIFT_END)
                    GameEvents.ShiftEnded();
            }

            _lastWholeHour = wholeHour;
        }

        public void SetTime(float targetHour, float lerpDuration = 2f)
        {
            if (targetHour < 0f || targetHour >= 24f)
            {
                Debug.LogWarning($"Invalid hour: {targetHour}. Must be between 0 and 24.");
                return;
            }

            if (targetHour < Hour)
                targetHour += 24f;

            StopCoroutine(nameof(LerpTime));
            StartCoroutine(LerpTime(targetHour, lerpDuration));
        }

        int _lastWholeHour = -1;

        // ─────────────────── Time Queries ───────────────────
        // TODO: Eventually query worker specific shift times
        public bool IsShiftTime(string workerId) =>
            Hour >= SHIFT_START && Hour < SHIFT_END;

        // ─────────────────── Helper Methods ───────────────────
        private System.Collections.IEnumerator LerpTime(float targetHour, float duration)
        {
            float startHour = Hour;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                Hour = Mathf.Lerp(startHour, targetHour, t);

                // day rollover
                if (Hour >= 24f)
                    Hour -= 24f;

                yield return null;
            }

            Hour = targetHour >= 24f ? targetHour - 24f : targetHour;
        }
    }
}
