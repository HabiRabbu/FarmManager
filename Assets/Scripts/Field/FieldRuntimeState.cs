using UnityEngine;

namespace Harvey.Farm.Fields
{
    public class FieldRuntimeState : MonoBehaviour
    {
        FieldTile[] tiles;
        int tilesCompleted = 0;

        public float Completion => tiles == null || tiles.Length == 0 ? 0 : (float)tilesCompleted / tiles.Length;
        
        public int TilesCompleted => tilesCompleted;

        public void Initialize(FieldTile[] t)
        {
            if (t == null)
            {
                Debug.LogWarning($"⚠️ FieldRuntimeState: Initialize called with null tiles array on {gameObject.name}");
                tiles = new FieldTile[0];
            }
            else
            {
                tiles = t;
                Debug.Log($"✅ FieldRuntimeState: Initialized with {tiles.Length} tiles on {gameObject.name}");
            }

            tilesCompleted = 0;
        }

        public void ResetProgress() 
        {
            tilesCompleted = 0;
            Debug.Log($"🔄 FieldRuntimeState: Progress reset on {gameObject.name}");
        }

        public void SetProgress(int completedCount)
        {
            if (tiles == null)
            {
                Debug.LogError($"❌ FieldRuntimeState: Cannot set progress - tiles array is null on {gameObject.name}");
                return;
            }

            tilesCompleted = Mathf.Clamp(completedCount, 0, tiles.Length);
        }

        public bool Advance()
        {
            if (tiles == null)
            {
                Debug.LogError($"❌ FieldRuntimeState: Cannot advance - tiles array is null on {gameObject.name}. Make sure Initialize() was called.");
                return false;
            }

            if (tiles.Length == 0)
            {
                Debug.LogWarning($"⚠️ FieldRuntimeState: Cannot advance - tiles array is empty on {gameObject.name}");
                return false;
            }

            tilesCompleted++;
            bool isComplete = tilesCompleted >= tiles.Length;
            
            return isComplete;
        }

        // Debug method to check state
        [ContextMenu("Debug State")]
        public void DebugState()
        {
            Debug.Log($"🔍 FieldRuntimeState Debug - GameObject: {gameObject.name}");
            Debug.Log($"  - Tiles: {(tiles == null ? "NULL" : tiles.Length.ToString())}");
            Debug.Log($"  - Completed: {tilesCompleted}");
            Debug.Log($"  - Completion: {Completion:P1}");
        }
    }
}