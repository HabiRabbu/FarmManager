using UnityEngine;
using UnityEngine.InputSystem;
using Harvey.Farm.Utilities;

namespace Harvey.Farm.InputScripts
{
    public class DebugInput : MonoBehaviour
    {

        private FarmInput input;

        void Awake()
        {
            input = new FarmInput();
        }
        void OnEnable()
        {
            input.Debug.Enable();
            input.Debug.ToggleDebug.performed += OnToggleDebug;
        }
        void OnDisable()
        {
            input.Debug.ToggleDebug.performed -= OnToggleDebug;
            input.Debug.Disable();
        }

        void OnToggleDebug(InputAction.CallbackContext ctx)
        {
            DebugManager.Instance.ToggleDebug();
        }
    }
}