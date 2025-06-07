using Harvey.Farm.CameraScripts;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraBoundsGizmo))]
public class CameraBoundsGizmoEditor : Editor
{
    private void OnSceneGUI()
    {
        CameraBoundsGizmo gizmo = (CameraBoundsGizmo)target;
        if (gizmo.CameraConfig == null || gizmo.CameraTarget == null) return;

        Handles.color = Color.cyan;

        var config = gizmo.CameraConfig;
        var y = gizmo.CameraTarget.position.y;

        Vector3 center = new Vector3(
            (config.MinPanX + config.MaxPanX) / 2f,
            y,
            (config.MinPanZ + config.MaxPanZ) / 2f
        );

        Vector3 minXPos = new Vector3(config.MinPanX, y, center.z);
        Vector3 maxXPos = new Vector3(config.MaxPanX, y, center.z);
        Vector3 minZPos = new Vector3(center.x, y, config.MinPanZ);
        Vector3 maxZPos = new Vector3(center.x, y, config.MaxPanZ);

        EditorGUI.BeginChangeCheck();

        minXPos = Handles.Slider(minXPos, Vector3.left);
        maxXPos = Handles.Slider(maxXPos, Vector3.right);
        minZPos = Handles.Slider(minZPos, Vector3.back);
        maxZPos = Handles.Slider(maxZPos, Vector3.forward);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(config, "Adjust Camera Bounds");

            config.MinPanX = minXPos.x;
            config.MaxPanX = maxXPos.x;
            config.MinPanZ = minZPos.z;
            config.MaxPanZ = maxZPos.z;

            EditorUtility.SetDirty(config);
        }
    }
}
