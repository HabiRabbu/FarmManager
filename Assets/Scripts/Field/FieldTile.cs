using UnityEngine;

public class FieldTile : MonoBehaviour
{
    public int gridX;
    public int gridZ;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetGridPosition(int x, int z)
    {
        gridX = x;
        gridZ = z;
    }
}
