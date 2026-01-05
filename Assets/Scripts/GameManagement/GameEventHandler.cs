using UnityEngine;
using Harvey.Farm.Events;
using Harvey.Farm.Utilities;

namespace Harvey.Farm.Managers
{
    /// <summary>
    /// Handles all game events related to the GameManager.
    /// This class manages event subscriptions and responses to keep GameManager focused on state management.
    /// </summary>
    public class GameEventHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameManager gameManager;
        [SerializeField] private PrefabPreloader prefabPreloader;

        private void Awake()
        {
            if (gameManager == null)
            {
                gameManager = GetComponent<GameManager>();
                if (gameManager == null)
                {
                    gameManager = GameManager.Instance;
                }
            }

            if (prefabPreloader == null)
            {
                prefabPreloader = GetComponent<PrefabPreloader>();
                if (prefabPreloader == null && gameManager != null)
                {
                    prefabPreloader = gameManager.GetComponent<PrefabPreloader>();
                }
            }
        }

        private void OnEnable()
        {
            GameEvents.OnPreloadComplete += HandlePreloadComplete;
            GameEvents.OnPreloadProgress += HandlePreloadProgress;

            GameEvents.OnOptionsMenuOpened += HandleOptionsMenuOpened;
            GameEvents.OnOptionsMenuClosed += HandleOptionsMenuClosed;
        }
        private void OnDisable()
        {
            GameEvents.OnPreloadComplete -= HandlePreloadComplete;
            GameEvents.OnPreloadProgress -= HandlePreloadProgress;

            GameEvents.OnOptionsMenuOpened -= HandleOptionsMenuOpened;
            GameEvents.OnOptionsMenuClosed -= HandleOptionsMenuClosed;
        }
        private void OnDestroy()
        {
            GameEvents.OnPreloadComplete -= HandlePreloadComplete;
            GameEvents.OnPreloadProgress -= HandlePreloadProgress;

            GameEvents.OnOptionsMenuOpened -= HandleOptionsMenuOpened;
            GameEvents.OnOptionsMenuClosed -= HandleOptionsMenuClosed;
        }

        #region Event Handlers
        private void HandlePreloadComplete()
        {
            Debug.Log("✅ GameEventHandler: Preload complete event received!");
        }

        private void HandlePreloadProgress(float progress)
        {
            Debug.Log($"🔄 GameEventHandler: Preload progress: {progress:P0}");
        }

        private void HandleOptionsMenuOpened()
        {
            if (gameManager != null && gameManager.CurrentState == GameState.Playing)
            {
                Debug.Log("🎮 GameEventHandler: Options menu opened - pausing game");
                gameManager.SetState(GameState.Paused);
            }
        }

        private void HandleOptionsMenuClosed()
        {
            if (gameManager != null && gameManager.CurrentState == GameState.Paused)
            {
                Debug.Log("🎮 GameEventHandler: Options menu closed - resuming game");
                gameManager.SetState(GameState.Playing);
            }
        }
        #endregion
    }
}