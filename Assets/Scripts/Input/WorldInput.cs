using UnityEngine;
using Harvey.Farm.Fields;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Harvey.Farm.UI;
using Harvey.Farm.Events;
using Harvey.Farm.Buildings;

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
            input = new FarmInput();
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
                // Missed everything on the ground layer: deselect field
                FieldManager.Instance.SelectField(null);
                return;
            }

            // ---------- Building ----------
            var building = hit.collider.GetComponent<Building>();
            if (building != null)
            {
                Debug.Log($"Clicked building: {building.DisplayName}");
                UIManager.Instance.OpenBuildingInfo(building);
                return;
            }

            // ---------- Field ----------
            FieldController clickedField = FieldManager.Instance.GetFieldAtPoint(hit.point);
            if (clickedField != null)
            {
                FieldManager.Instance.SelectField(clickedField);
                return;
            }

            // ---------- Missed everything ----------
            FieldManager.Instance.SelectField(null);
            UIManager.Instance.OpenBuildingInfo(null);
        }

    }
}
