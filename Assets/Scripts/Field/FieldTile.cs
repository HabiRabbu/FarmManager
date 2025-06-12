using TMPro;
using UnityEngine;

namespace Harvey.Farm.FieldScripts
{
    [RequireComponent(typeof(MeshRenderer))]
    public class FieldTile : MonoBehaviour
    {

        //DEBUG
        [SerializeField] private TMP_Text debugTextLabel;



        [SerializeField] private Material earthMat;
        [SerializeField] private Material plowedMat;

        [SerializeField] public int GridX { get; private set; }
        [SerializeField] public int GridZ { get; private set; }

        public bool IsPlowed { get; private set; }

        private MeshRenderer rndr;

        public Vector3 WorldPosition => transform.position;

        public void Init(int x, int z)
        {
            GridX = x;
            GridZ = z;
            rndr = GetComponent<MeshRenderer>();
            rndr.material = earthMat;
            IsPlowed = false;

            //Debug
            debugTextLabel.text = "(" + GridX + "," + GridZ + ")";
        }

        public void Plow()
        {
            if (IsPlowed) return;
            IsPlowed = true;
            rndr.material = plowedMat;
        }
    }
}

