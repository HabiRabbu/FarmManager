using UnityEngine;
using Harvey.Farm.FieldScripts;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Harvey.Farm.UI;
using Harvey.Farm.Events;

namespace Harvey.Farm.InputScripts
{
    public class WorldInput : MonoBehaviour
    {
        [Header("Raycast Settings")]
        [SerializeField] private LayerMask groundLayer;
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

            if (!Physics.Raycast(ray, out RaycastHit hit, maxRayDist, groundLayer))
            {
                FieldManager.Instance.SelectField(null);
                return;
            }

            Field clickedField = FieldManager.Instance.GetFieldAtPoint(hit.point);
            FieldManager.Instance.SelectField(clickedField);
        }
    }
}
