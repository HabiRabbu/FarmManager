#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TakeScreenshot))]
public class TakeScreenshotEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TakeScreenshot ts = (TakeScreenshot)target;

        EditorGUILayout.Space();

        // The actual button
        if (GUILayout.Button("Take Screenshot"))
        {
            ts.TakeScreenshotNow();
        }
    }
}
#endif
