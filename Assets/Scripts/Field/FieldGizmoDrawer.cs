#if UNITY_EDITOR
using Harvey.Farm.Fields;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class FieldGizmoDrawer : MonoBehaviour
{
    public FieldDefinition definition;
    FieldBuilder builder;

    void OnDrawGizmos()
    {
        if (definition == null)
        {
            var controller = GetComponent<FieldController>();
            if (controller != null)
                definition = controller.definition;
        }
        if (definition == null) return;

        Vector3 centre = transform.position;
        float halfW = definition.width * definition.tileSize * 0.5f;
        float halfH = definition.height * definition.tileSize * 0.5f;
        float y = centre.y;

        Gizmos.color = Color.green;
        Vector3 bl = new Vector3(centre.x - halfW, y, centre.z - halfH);
        Vector3 br = new Vector3(centre.x + halfW, y, centre.z - halfH);
        Vector3 tl = new Vector3(centre.x - halfW, y, centre.z + halfH);
        Vector3 tr = new Vector3(centre.x + halfW, y, centre.z + halfH);

        Gizmos.DrawLine(tl, tr); Gizmos.DrawLine(tr, br);
        Gizmos.DrawLine(br, bl); Gizmos.DrawLine(bl, tl);

        for (int x = 1; x < definition.width; x++)
        {
            float xPos = centre.x - halfW + x * definition.tileSize;
            Gizmos.DrawLine(new Vector3(xPos, y, centre.z - halfH), new Vector3(xPos, y, centre.z + halfH));
        }

        for (int z = 1; z < definition.height; z++)
        {
            float zPos = centre.z - halfH + z * definition.tileSize;
            Gizmos.DrawLine(new Vector3(centre.x - halfW, y, zPos), new Vector3(centre.x + halfW, y, zPos));
        }
    }
    void OnValidate()
    {
#if UNITY_EDITOR
        UnityEditor.SceneView.RepaintAll();
#endif
    }
}
#endif


