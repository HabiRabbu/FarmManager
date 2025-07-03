using UnityEngine;
using UnityEngine.Animations;

namespace Harvey.Farm.CameraScripts
{
    [CreateAssetMenu(fileName = "CameraConfigSO", menuName = "Roast/Camera/CameraConfigSO")]
    public class CameraConfig : ScriptableObject
    {

        [field: Header("Pan Settings")]
        [field: SerializeField] public bool EnableEdgePan { get; private set; } = true;
        [field: SerializeField] public float MousePanSpeed { get; private set; } = 15;
        [field: SerializeField] public float EdgePanSize { get; private set; } = 50;
        [field: SerializeField] public float KeyboardPanSpeed { get; private set; } = 15;

        [field: Header("Pan Boundaries")]
        [field: SerializeField] public float MinPanX { get; set; } = -100f;
        [field: SerializeField] public float MaxPanX { get; set; } = 100f;
        [field: SerializeField] public float MinPanZ { get; set; } = -100f;
        [field: SerializeField] public float MaxPanZ { get; set; } = 100f;

        [field: Header("Zoom Settings")]
        [field: SerializeField] public float ZoomSpeed { get; private set; } = 15f;
        [field: SerializeField] public float ScrollWheelMultiplier { get; private set; } = 15f;
        [field: SerializeField] public float MinZoomDistance { get; private set; } = 1f;
        [field: SerializeField] public float MaxZoomDistance { get; private set; } = 20f;

        [field: Header("Rotation Settings")]
        [field: SerializeField] public float RotationSpeed { get; private set; } = 100f;
        [field: SerializeField] public Vector2 RotationClamp { get; private set; } = new Vector2(-50f, 20f);
    }
}
