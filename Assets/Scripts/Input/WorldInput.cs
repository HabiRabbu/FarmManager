using UnityEngine;
using Harvey.Farm.FieldScripts;
using UnityEngine.InputSystem;
using System;

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
            Vector2 sp = input.World.Pointer.ReadValue<Vector2>();
            Ray ray = cam.ScreenPointToRay(sp);

            if (!Physics.Raycast(ray, out RaycastHit hit, maxRayDist, groundLayer))
                return;

            var field = FieldManager.Instance.GetFieldAtPoint(hit.point);
            field?.HandleTileClick(hit.point);
        }
    }
}
