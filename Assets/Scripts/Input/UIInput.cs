using Harvey.Farm.Events;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Harvey.Farm.InputScripts
{
    public class UIInput : MonoBehaviour
    {

        private FarmInput input;

        void Awake()
        {
            input = InputService.Instance.Actions;
        }
        void OnEnable()
        {
            input.UI.Enable();
            input.UI.Escape.performed += OnEscapePressed;
        }
        void OnDisable()
        {
            input.UI.Escape.performed -= OnEscapePressed;
            input.UI.Disable();
        }

        private void OnEscapePressed(InputAction.CallbackContext ctx)
        {
            GameEvents.CloseAllUI();
        }
    }
}