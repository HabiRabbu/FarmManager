using UnityEngine;

namespace Harvey.Farm.Fields
{
    public class FieldRuntimeState : MonoBehaviour
    {
        FieldTile[] tiles;
        int tilesCompleted = 0;

        public float Completion => tiles == null ? 0 : (float)tilesCompleted / tiles.Length;

        public void Initialize(FieldTile[] t)
        {
            tiles = t;
            tilesCompleted = 0;
        }

        public void ResetProgress() => tilesCompleted = 0;

        public bool Advance()
        {
            tilesCompleted++;
            return tilesCompleted >= tiles.Length;
        }
    }
}