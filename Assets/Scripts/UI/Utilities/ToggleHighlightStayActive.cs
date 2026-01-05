namespace Harvey.Farm.UI.Utilities
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// This script is used to keep the highlight of a toggle active when the toggle is clicked.
    /// </summary>
    public class ToggleHighlightStayActive : MonoBehaviour
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private Image imageToKeepFocused;

        private void Reset()
        {
            toggle = GetComponent<Toggle>();
        }
        private void Awake()
        {
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
            OnToggleValueChanged(toggle.isOn);
        }

        void OnDestroy()
        {
            toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
        }

        private void OnToggleValueChanged(bool isOn)
        {
            if (imageToKeepFocused == null) return;

            imageToKeepFocused.color = toggle.isOn ? toggle.colors.selectedColor : Color.clear;
        }
    }
}