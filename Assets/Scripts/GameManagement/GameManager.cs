using UnityEngine;
using System.Threading.Tasks;
using Harvey.Farm.Utilities;
using Harvey.Farm.Events;
using Unity.VisualScripting;
using System;

namespace Harvey.Farm.Managers
{
    public enum GameState
    {
        Preloading,
        Ready,
        Playing,
        Paused
    }

    [DefaultExecutionOrder(-1000)]
    public class GameManager : Singleton<GameManager>
    {
        [Header("Components")]
        [SerializeField] private PrefabPreloader prefabPreloader;
        
        [Header("Event Handling")]
        [Tooltip("GameEventHandler component should be attached to this GameObject or a child")]
        [SerializeField] private GameEventHandler eventHandler;

        [Header("Game State")]
        [SerializeField] private GameState currentState = GameState.Preloading;

        public GameState CurrentState => currentState;
        public bool IsGameReady => currentState != GameState.Preloading;
        public bool IsPreloadComplete => prefabPreloader?.IsPreloadComplete ?? false;

        protected override void Awake()
        {
            base.Awake();
            
            if (prefabPreloader == null)
            {
                prefabPreloader = GetComponent<PrefabPreloader>();
            }
            
            if (eventHandler == null)
            {
                eventHandler = GetComponent<GameEventHandler>();
                if (eventHandler == null)
                {
                    // Create GameEventHandler if it doesn't exist
                    eventHandler = gameObject.AddComponent<GameEventHandler>();
                    Debug.Log("🎮 GameManager: Created GameEventHandler component automatically");
                }
            }
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



        private void OnGameReady()
        {
            if (currentState != GameState.Preloading) return;

            SetState(GameState.Ready);
            Debug.Log("🎮 GameManager: Game is ready! All spawning will now be instant.");

            GameEvents.GameReady();

            // TODO: Hide loading screen and show main game UI
            // UIManager.Instance.HideLoadingScreen();
            // UIManager.Instance.ShowMainGameUI();

            SetState(GameState.Playing);
            GameEvents.GameIsPlaying();
        }

        public void SetState(GameState newState)
        {
            if (currentState == newState) return;

            var previousState = currentState;
            currentState = newState;

            Debug.Log($"🎮 GameManager: State changed from {previousState} to {newState}");

            // Handle state-specific logic
            switch (newState)
            {
                case GameState.Preloading:
                    break;
                case GameState.Ready:
                    break;
                case GameState.Playing:
                    Time.timeScale = 1f;
                    break;
                case GameState.Paused:
                    Time.timeScale = 0f;
                    break;
            }

            // GameEvents.OnGameStateChanged?.Invoke(previousState, newState);
        }

        public void TogglePause()
        {
            if (currentState == GameState.Playing)
                SetState(GameState.Paused);
            else if (currentState == GameState.Paused)
                SetState(GameState.Playing);
        }
    }
}