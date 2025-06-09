using UnityEngine;

namespace Harvey.Farm.FieldScripts
{
    public class TileGrid
    {
        public readonly int Width, Height;
        public readonly float TileSize;
        public readonly Vector3 Origin;      // world-space center
        private readonly Vector2[] centers;  // flattened array of XZ centers

        public TileGrid(int width, int height, float tileSize, Vector3 origin)
        {
            Width = width;
            Height = height;
            TileSize = tileSize;
            Origin = origin;

            centers = new Vector2[width * height];
            float xOff = -((width - 1) * tileSize) / 2f;
            float zOff = -((height - 1) * tileSize) / 2f;

            int i = 0;
            for (int x = 0; x < width; x++)
                for (int z = 0; z < height; z++)
                {
                    float wx = origin.x + x * tileSize + xOff;
                    float wz = origin.z + z * tileSize + zOff;
                    centers[i++] = new Vector2(wx, wz);
                }
        }

        public bool Contains(Vector3 worldPos)
        {
            Vector3 local = worldPos - Origin;
            float halfW = Width * TileSize * 0.5f;
            float halfH = Height * TileSize * 0.5f;
            return Mathf.Abs(local.x) <= halfW
                && Mathf.Abs(local.z) <= halfH;
        }

        public int IndexOfNearest(Vector3 worldPos)
        {
            Vector2 wp = new Vector2(worldPos.x, worldPos.z);
            int bestI = 0;
            float bestDs = float.MaxValue;
            for (int i = 0; i < centers.Length; i++)
            {
                float d = Vector2.SqrMagnitude(centers[i] - wp);
                if (d < bestDs)
                {
                    bestDs = d;
                    bestI = i;
                }
            }
            return bestI;
        }

        /// <summary>
        /// Instantiate prefabs at each tile centre, parented under `parent`,
        /// set their localScale to (tileSize, y, tileSize),
        /// and return a flat array of their Tile scripts.
        /// </summary>
        public FieldTile[] Generate(GameObject tilePrefab, Transform parent, float tileYScale = 0.2f)
        {
            var tiles = new FieldTile[centers.Length];
            for (int i = 0; i < centers.Length; i++)
            {
                Vector3 pos = new Vector3(centers[i].x, parent.position.y, centers[i].y);
                var go = Object.Instantiate(tilePrefab, pos, Quaternion.identity, parent);
                go.transform.localScale = new Vector3(TileSize, tileYScale, TileSize);
                go.name = $"Tile_{i}";
                tiles[i] = go.GetComponent<FieldTile>();
                tiles[i].Init(i % Width, i / Width);
            }
            return tiles;
        }
    }
}