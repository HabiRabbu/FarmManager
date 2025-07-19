using UnityEngine;
using System.Threading.Tasks;
using Harvey.Farm.Utilities;
using Harvey.Farm.Events;
using Unity.VisualScripting;

namespace Harvey.Farm.Managers
{
    /// <summary>
    /// Main game manager that handles game state and coordinates with PrefabPreloader.
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public class GameManager : Singleton<GameManager>
    {
        [Header("Components")]
        [SerializeField] private PrefabPreloader prefabPreloader;

        [Header("Game State")]
        [SerializeField] private bool isGameReady = false;

        public bool IsGameReady => isGameReady;
        public bool IsPreloadComplete => prefabPreloader?.IsPreloadComplete ?? false;

        protected override void Awake()
        {
            base.Awake();
            
            if (prefabPreloader == null)
            {
                prefabPreloader = GetComponent<PrefabPreloader>();
            }
        }

        void OnEnable()
        {
            GameEvents.OnPreloadComplete += OnPreloadComplete;
            GameEvents.OnPreloadProgress += OnPreloadProgress;
        }
        void OnDisable()
        {
            GameEvents.OnPreloadComplete -= OnPreloadComplete;
            GameEvents.OnPreloadProgress -= OnPreloadProgress;
        }

        async void Start()
        {
            Debug.Log("🎮 GameManager: Starting game initialization...");

            // Wait for preloading to complete
            if (prefabPreloader != null && !prefabPreloader.IsPreloadComplete)
            {
                Debug.Log("⏳ GameManager: Waiting for asset preloading...");
                await WaitForPreloadComplete();
            }

            OnGameReady();
        }

        private async Task WaitForPreloadComplete()
        {
            while (prefabPreloader != null && !prefabPreloader.IsPreloadComplete)
            {
                await Task.Yield();
            }
        }

        private void OnPreloadComplete()
        {
            Debug.Log("✅ GameManager: Preload complete!");
            OnGameReady();
        }

        private void OnPreloadProgress(float progress)
        {
            // TODO: Update loading screen progress
            // UIManager.Instance.UpdateLoadingProgress(progress);
        }

        private void OnGameReady()
        {
            if (isGameReady) return;

            isGameReady = true;
            Debug.Log("🎮 GameManager: Game is ready! All spawning will now be instant.");

            // Broadcast game ready event
            GameEvents.GameReady();

            // TODO: Hide loading screen and show main game UI
            // UIManager.Instance.HideLoadingScreen();
            // UIManager.Instance.ShowMainGameUI();
        }
    }
}