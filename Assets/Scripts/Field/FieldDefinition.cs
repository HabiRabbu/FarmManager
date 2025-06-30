using UnityEngine;

[CreateAssetMenu(menuName = "Farm/Field Definition")]
public class FieldDefinition : ScriptableObject
{
    public string fieldName;
    public int width;
    public int height;
    public float tileSize = 1f;
    public GameObject tilePrefab;
}
