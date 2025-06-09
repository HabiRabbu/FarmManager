using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class FieldTile : MonoBehaviour
{
    [SerializeField] private Material earthMat;
    [SerializeField] private Material plowedMat;

    public bool IsPlowed { get; private set; }

    private MeshRenderer rndr;
    public int GridX { get; private set; }
    public int GridZ { get; private set; }

    public void Init(int x, int z)
    {
        GridX = x;
        GridZ = z;
        rndr = GetComponent<MeshRenderer>();
        rndr.material = earthMat;
        IsPlowed = false;
    }

    public void Plow()
    {
        if (IsPlowed) return;
        IsPlowed = true;
        rndr.material = plowedMat;
    }
}
