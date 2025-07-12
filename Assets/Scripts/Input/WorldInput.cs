using UnityEngine;
using Harvey.Farm.Fields;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Harvey.Farm.UI;
using Harvey.Farm.Events;
using Harvey.Farm.Buildings;
using Harvey.Farm.UI.Radial;

namespace Harvey.Farm.InputScripts
{
    public class WorldInput : MonoBehaviour
    {
        [Header("Raycast Settings")]
        [SerializeField] private LayerMask clickableLayerMask;
        [SerializeField, Min(1f)] float maxRayDist = 100f;

        private Camera cam;
        private FarmInput input;

        void Awake()
        {
            cam = Camera.main;
            input = InputService.Instance.Actions;
        }

        void OnEnable()
        {
            input.World.Enable();
            input.World.WorldSelect.performed += OnSelect;
        }
        void OnDisable()
        {
            input.World.WorldSelect.performed -= OnSelect;
            input.World.Disable();
        }

        void OnSelect(InputAction.CallbackContext ctx)
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            Vector2 screenPos = input.World.Pointer.ReadValue<Vector2>();
            Ray ray = cam.ScreenPointToRay(screenPos);
            if (!Physics.Raycast(ray, out RaycastHit hit, maxRayDist, clickableLayerMask))
            {
                // Missed everything
                UIManager.Instance.CloseAll();
                return;
            }

            // Radial on generic launcher - Building, Vehicle, etc.
            if (hit.collider.TryGetComponent<RadialLauncher>(out var launcher))
            {
                UIManager.Instance.ShowRadial(launcher, screenPos);
                return;
            }

            // Field hit? (Handled differently - GetFieldAtPoint)
            FieldController field = FieldManager.Instance.GetFieldAtPoint(hit.point);
            if (field != null)
            {
                if (field.TryGetComponent<RadialLauncher>(out var fieldLauncher))
                    UIManager.Instance.ShowRadial(fieldLauncher, screenPos);

                return;
            }

            // Nothing hit?
            UIManager.Instance.CloseAll();
        }
    }
}
