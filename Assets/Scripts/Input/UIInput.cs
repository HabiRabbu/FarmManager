using UnityEngine;

namespace Harvey.Farm.InputScripts
{
    public class UIInput : MonoBehaviour
    {

        private FarmInput input;

        void Awake()
        {
            input = new FarmInput();
        }
         void OnEnable()
        {
            // example - input.Debug.Enable();
            // example - input.Debug.ToggleDebug.performed += OnToggleDebug;
        }
        void OnDisable()
        {
            // example - input.Debug.ToggleDebug.performed -= OnToggleDebug;
            // example - input.Debug.Disable();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}