using TMPro;
using UnityEngine;
using Harvey.Farm.Events;
using Harvey.Farm.Factory;
using Harvey.Data.Coffee;

namespace Harvey.Farm.Fields
{
    public class FieldTile : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private Material earthMat;
        [SerializeField] private Material plowedMat;
        public Transform cropAnchor;
        GameObject cropInstance;
        CoffeeCropData currentCrop;
        int currentStage = -1;

        [SerializeField] public int GridX { get; private set; }
        [SerializeField] public int GridZ { get; private set; }

        public bool IsPlowed { get; private set; } = false;
        public bool IsSeeded { get; private set; } = false;
        public bool IsHarvested { get; private set; } = false;

        public bool IsReserved { get; private set; } = false;

        private MeshRenderer rndr;

        public Vector3 WorldPosition => transform.position;

        void Awake() => rndr = GetComponentInChildren<MeshRenderer>();

        public void Init(int x, int z)
        {
            GridX = x;
            GridZ = z;
            rndr = GetComponentInChildren<MeshRenderer>();
            rndr.sharedMaterial = earthMat;
            IsPlowed = false;
        }

        public bool TryReserve()
        {
            if (IsReserved) return false;
            IsReserved = true;
            return true;
        }
        public void ClearReservation() => IsReserved = false;

        public void Plow()
        {
            if (IsPlowed) return;
            IsPlowed = true;
            IsHarvested = false;
            IsSeeded = false;

            rndr.sharedMaterial = plowedMat;

            GameEvents.TilePlowed(this);
        }

        public void Seed(CoffeeCropData crop)
        {
            if (IsSeeded) return;

            IsSeeded = true;
            currentCrop = crop;
            SpawnOrGetCrop();
            SetStage(0);

            GameEvents.TileSeeded(this, crop);
        }
        public void SetStage(int stage)
        {
            if (!IsSeeded || currentStage == stage) return;

            var meshes = cropInstance.transform.GetChild(0); // “Meshes”
            for (int i = 0; i < meshes.childCount; i++)
                meshes.GetChild(i).gameObject.SetActive(i == stage);

            currentStage = stage;
        }

        public void Harvest()
        {
            if (IsHarvested) return;

            rndr.sharedMaterial = earthMat;
            IsHarvested = true;
            IsPlowed = false;
            IsSeeded = false;

            CropFactory.Instance.Despawn(currentCrop, cropInstance);
            cropInstance = null;
            currentCrop = null;
            currentStage = -1;

            GameEvents.TileHarvested(this);
        }

        public void ResetTile()
        {
            if (cropInstance != null)
            {
                CropFactory.Instance.Despawn(currentCrop, cropInstance);
                cropInstance = null;
            }

            IsPlowed = false;
            IsSeeded = false;
            IsHarvested = false;
            IsReserved = false;
            
            currentCrop = null;
            currentStage = -1;

            rndr.sharedMaterial = earthMat;
        }

        void SpawnOrGetCrop()
        {
            if (cropInstance != null) return;

            cropInstance = CropFactory.Instance.Spawn(
                currentCrop,
                cropAnchor,
                Vector3.zero
            );
        }
    }
}

