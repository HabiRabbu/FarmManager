using UnityEngine;
using UnityEngine.InputSystem;

public class InputService : MonoBehaviour
{
    public static InputService Instance { get; private set; }
    public FarmInput Actions { get; private set; }

    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance  = this;
        Actions   = new FarmInput();
        DontDestroyOnLoad(gameObject);
    }
}
