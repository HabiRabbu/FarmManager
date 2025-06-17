using TMPro;
using UnityEngine;
using Harvey.Farm.Events;

namespace Harvey.Farm.FieldScripts
{
    [RequireComponent(typeof(MeshRenderer))]
    public class FieldTile : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private TMP_Text debugTextLabel;
        void OnEnable() => DebugEvents.OnDebugModeToggled += SetLabelVisible;
        void OnDisable() => DebugEvents.OnDebugModeToggled -= SetLabelVisible;


        [SerializeField] private Material earthMat;
        [SerializeField] private Material plowedMat;

        [SerializeField] public int GridX { get; private set; }
        [SerializeField] public int GridZ { get; private set; }

        public bool IsPlowed { get; private set; } = false;
        public bool IsSeeded { get; private set; } = false;
        public bool IsHarvested { get; private set; } = false;

        private MeshRenderer rndr;

        public Vector3 WorldPosition => transform.position;

        void Awake() => rndr = GetComponent<MeshRenderer>();

        public void Init(int x, int z)
        {
            GridX = x;
            GridZ = z;
            rndr = GetComponent<MeshRenderer>();
            rndr.sharedMaterial = earthMat;
            IsPlowed = false;

            //Debug
            if (debugTextLabel != null) { debugTextLabel.text = $"({GridX},{GridZ})"; }
            if (debugTextLabel.gameObject.activeSelf) { SetLabelVisible(false); }
        }

        public void Plow()
        {
            if (IsPlowed) return;
            IsPlowed = true;
            rndr.material = plowedMat;

            GameEvents.TilePloughed(this);
        }

        private void SetLabelVisible(bool show)
        {
            if (debugTextLabel != null)
                debugTextLabel.gameObject.SetActive(show);
        }
    }
}

