using UnityEngine;

/// <summary>
/// Drop-in generic singleton. Attach to exactly one GameObject
/// (or create none and the first `GetOrCreate()` call will make one).
/// </summary>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this as T;

        // Genuinely need persistence across scenes - Uncomment this line. Not rn though
        // DontDestroyOnLoad(gameObject);
    }
}
