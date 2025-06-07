using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Harvey.Farm.CameraScripts
{
    public class CameraInput : MonoBehaviour
    {
        [SerializeField] private Transform cameraTarget;
        [SerializeField] private CinemachineCamera cinemachineCamera;
        [SerializeField] private new Camera camera;
        [SerializeField] private CameraConfig cameraConfig;

        private CinemachineFollow cinemachineFollow;
        private float currentYaw = 0f;
        private float currentPitch = 20f;

        void Awake()
        {
            if (!cinemachineCamera.TryGetComponent(out cinemachineFollow))
            {
                Debug.LogError("Cinemachine Camera did not have CinemachineFollow! Zoom is broken...");
            }
        }

        private void Update()
        {
            HandlePan();
            HandleZoom();
            HandleRotation();
        }

        private void HandlePan()
        {
            Vector2 moveAmount = Vector2.zero;
            moveAmount = GetKeyboardMoveAmount();
            moveAmount += GetMouseMoveAmount();

            Vector3 forward = cameraTarget.forward;
            Vector3 right = cameraTarget.right;

            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            Vector3 move = forward * moveAmount.y + right * moveAmount.x;

            cameraTarget.position += move;

            Vector3 clampedPosition = cameraTarget.position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, cameraConfig.MinPanX, cameraConfig.MaxPanX);
            clampedPosition.z = Mathf.Clamp(clampedPosition.z, cameraConfig.MinPanZ, cameraConfig.MaxPanZ);
            cameraTarget.position = clampedPosition;
        }

        private Vector2 GetMouseMoveAmount()
        {
            Vector2 moveAmount = Vector2.zero;

            if (!cameraConfig.EnableEdgePan) { return moveAmount; }

            Vector2 mousePosition = Mouse.current.position.ReadValue();
            int screenWidth = Screen.width;
            int screenHeight = Screen.height;

            if (mousePosition.x <= cameraConfig.EdgePanSize)
            {
                moveAmount.x -= 1f;
            }
            else if (mousePosition.x >= screenWidth - cameraConfig.EdgePanSize)
            {
                moveAmount.x += 1f;
            }

            if (mousePosition.y >= screenHeight - cameraConfig.EdgePanSize)
            {
                moveAmount.y += 1f;
            }
            else if (mousePosition.y <= cameraConfig.EdgePanSize)
            {
                moveAmount.y -= 1f;
            }

            moveAmount = moveAmount.normalized * cameraConfig.MousePanSpeed * Time.deltaTime;
            return moveAmount;
        }

        private Vector2 GetKeyboardMoveAmount()
        {
            Vector2 moveAmount = Vector2.zero;
            if (Keyboard.current.upArrowKey.isPressed || Keyboard.current.wKey.isPressed)
            {
                moveAmount.y += 1f;
            }
            if (Keyboard.current.downArrowKey.isPressed || Keyboard.current.sKey.isPressed)
            {
                moveAmount.y -= 1f;
            }
            if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed)
            {
                moveAmount.x -= 1f;
            }
            if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
            {
                moveAmount.x += 1f;
            }

            moveAmount = moveAmount.normalized * cameraConfig.KeyboardPanSpeed * Time.deltaTime;
            return moveAmount;
        }

        private void HandleRotation()
        {
            if (Mouse.current.rightButton.isPressed)
            {
                Vector2 mouseDelta = Mouse.current.delta.ReadValue();
                currentYaw += mouseDelta.x * cameraConfig.RotationSpeed * Time.deltaTime;
                currentPitch -= mouseDelta.y * cameraConfig.RotationSpeed * Time.deltaTime;
                currentPitch = Mathf.Clamp(currentPitch, cameraConfig.RotationClamp.x, cameraConfig.RotationClamp.y);

                Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
                cameraTarget.rotation = rotation;
            }
        }

        private void HandleZoom()
        {
            float zoomInput = 0f;

            zoomInput += Mouse.current.scroll.ReadValue().y * cameraConfig.ScrollWheelMultiplier;
            if (Keyboard.current.qKey.isPressed)
                zoomInput -= 1f;
            if (Keyboard.current.eKey.isPressed)
                zoomInput += 1f;

            if (Mathf.Abs(zoomInput) > 0.01f)
            {
                Vector3 followOffset = cinemachineFollow.FollowOffset;

                Vector3 zoomDirection = followOffset.normalized;
                float zoomAmount = zoomInput * cameraConfig.ZoomSpeed * Time.deltaTime;

                followOffset -= zoomDirection * zoomAmount;

                float distance = followOffset.magnitude;
                distance = Mathf.Clamp(distance, cameraConfig.MinZoomDistance, cameraConfig.MaxZoomDistance);
                followOffset = zoomDirection * distance;

                cinemachineFollow.FollowOffset = followOffset;
            }
        }
    }
}