using Harvey.Farm.Utilities;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Harvey.Farm.CameraScripts
{
    public class CameraInput : MonoBehaviour
    {
        [Header("Scene refs")]
        [SerializeField] Transform cameraTarget;
        [SerializeField] CameraConfig cameraConfig;
        [SerializeField] bool enableEdgePan = true;
        [SerializeField] CinemachineCamera cam;
        [SerializeField] CinemachineFollow follow;

        [Header("Speed multipliers")]
        [SerializeField] float normalPanMultiplier = 1f;
        [SerializeField] float shiftPanMultiplier = 2f;

        /* ─────────────────────────────────────────────── */

        FarmInput actions;

        float yaw = 0f;
        float pitch = 20f;

        void Awake()
        {
            actions = InputService.Instance.Actions;
        }

        void OnEnable() => actions.Camera.Enable();
        void OnDisable() => actions.Camera.Disable();

        void Update()
        {
            if (DebugManager.DebugOn) return;

            HandlePan();
            HandleZoom();
            HandleRotation();
        }

        /* ───────────────────────────── Pan ───────────── */
        void HandlePan()
        {
            Vector2 move = actions.Camera.Move.ReadValue<Vector2>();
            if (enableEdgePan) move += GetEdgePan();

            float speedMul = actions.Camera.Fast.IsPressed()
                           ? shiftPanMultiplier : normalPanMultiplier;

            move = move.normalized * cameraConfig.KeyboardPanSpeed
                                 * speedMul * Time.deltaTime;

            Vector3 fwd = cameraTarget.forward; fwd.y = 0; fwd.Normalize();
            Vector3 right = cameraTarget.right; right.y = 0; right.Normalize();

            cameraTarget.position += fwd * move.y + right * move.x;
            ClampPan();
        }

        Vector2 GetEdgePan()
        {
            if (!cameraConfig.EnableEdgePan) return Vector2.zero;

            Vector2 pos = Mouse.current.position.ReadValue();
            Vector2 pan = Vector2.zero;

            if (pos.x <= cameraConfig.EdgePanSize) pan.x -= 1;
            else if (pos.x >= Screen.width - cameraConfig.EdgePanSize) pan.x += 1;
            if (pos.y >= Screen.height - cameraConfig.EdgePanSize) pan.y += 1;
            else if (pos.y <= cameraConfig.EdgePanSize) pan.y -= 1;

            return pan * cameraConfig.MousePanSpeed * Time.deltaTime;
        }

        void ClampPan()
        {
            Vector3 p = cameraTarget.position;
            p.x = Mathf.Clamp(p.x, cameraConfig.MinPanX, cameraConfig.MaxPanX);
            p.z = Mathf.Clamp(p.z, cameraConfig.MinPanZ, cameraConfig.MaxPanZ);
            cameraTarget.position = p;
        }

        /* ─────────────────────────── Zoom ────────────── */
        void HandleZoom()
        {
            float zoomInput = actions.Camera.Zoom.ReadValue<float>();
            if (Mathf.Abs(zoomInput) < 0.01f) return;

            if (actions.Camera.Fast.IsPressed())
                zoomInput *= 3f;

            Vector3 off = follow.FollowOffset;
            Vector3 dir = off.normalized;

            off -= dir * zoomInput * cameraConfig.ZoomSpeed * Time.deltaTime;

            float dist = Mathf.Clamp(off.magnitude,
                                     cameraConfig.MinZoomDistance,
                                     cameraConfig.MaxZoomDistance);
            follow.FollowOffset = dir * dist;
        }

        /* ───────────────────────── Rotation ──────────── */
        void HandleRotation()
        {
            if (!actions.Camera.RotateHeld.IsPressed()) return;

            Vector2 delta = actions.Camera.Rotate.ReadValue<Vector2>();
            if (delta.sqrMagnitude < 0.001f) return;

            yaw += delta.x * cameraConfig.RotationSpeed * Time.deltaTime;
            pitch -= delta.y * cameraConfig.RotationSpeed * Time.deltaTime;
            pitch = Mathf.Clamp(pitch,
                                 cameraConfig.RotationClamp.x,
                                 cameraConfig.RotationClamp.y);

            cameraTarget.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }
    }
}
