using Unity.Cinemachine;
using UnityEngine;

#if UNITY_EDITOR
namespace Harvey.Farm.CameraScripts
{
    [ExecuteAlways]
    public class CameraBoundsGizmo : MonoBehaviour
    {
        [SerializeField] private CameraConfig cameraConfig;
        [SerializeField] private Transform cameraTarget;
        [SerializeField] private bool showGizmo = true;
        [SerializeField] private Color gizmoFillColor = new Color(0f, 1f, 1f, 0.15f);
        [SerializeField] private Color gizmoOutlineColor = new Color(0f, 1f, 1f, 1f);

        public CameraConfig CameraConfig => cameraConfig;
        public Transform CameraTarget => cameraTarget;


        private void OnDrawGizmos()
        {
            if (!showGizmo || cameraConfig == null || cameraTarget == null) return;

            float width = cameraConfig.MaxPanX - cameraConfig.MinPanX;
            float depth = cameraConfig.MaxPanZ - cameraConfig.MinPanZ;
            Vector3 center = new Vector3(
                (cameraConfig.MinPanX + cameraConfig.MaxPanX) / 2f,
                cameraTarget.position.y,
                (cameraConfig.MinPanZ + cameraConfig.MaxPanZ) / 2f
            );

            Vector3 size = new Vector3(width, 1f, depth);

            Gizmos.color = gizmoFillColor;
            Gizmos.DrawCube(center, size);

            Gizmos.color = gizmoOutlineColor;
            Gizmos.DrawWireCube(center, size);
        }
    }
}
#endif
