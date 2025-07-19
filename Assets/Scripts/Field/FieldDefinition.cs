using UnityEngine;

[CreateAssetMenu(menuName = "Roast/Fields/Field Definition")]
public class FieldDefinition : ScriptableObject
{
    public string FieldName;
    public int Width;
    public int Height;
    public float TileSize = 1f;
    public string TilePrefabGuid = "basic-field-tile"; // TODO: Avoid hardcoding by using definition data
    public string PrefabGuid = "basic-field"; // TODO: Avoid hardcoding by using definition data
}
