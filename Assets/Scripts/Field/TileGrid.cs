using System.Collections.Generic;
using UnityEngine;

namespace Harvey.Farm.FieldScripts
{
    public class TileGrid
    {
        public readonly int Width, Height;
        public readonly float TileSize;
        public readonly Vector3 Origin;      // world-space center
        private readonly Vector2[] centers;  // flattened array of XZ centers

        public FieldTile[] BuildSerpentineRows(FieldTile[] tiles)
        {
            var path = new List<FieldTile>(tiles.Length);

            for (int z = 0; z < Height; z++)
            {
                if ((z & 1) == 0)
                    for (int x = 0; x < Width; x++)
                        path.Add(tiles[z * Width + x]);
                else
                    for (int x = Width - 1; x >= 0; x--)
                        path.Add(tiles[z * Width + x]);
            }
            return path.ToArray();
        }

        public FieldTile[] BuildSerpentineColumns(FieldTile[] tiles)
        {
            var path = new List<FieldTile>(tiles.Length);

            for (int x = 0; x < Width; x++)
            {
                if ((x & 1) == 0)
                {
                    for (int z = Height - 1; z >= 0; z--)
                        path.Add(tiles[z * Width + x]);
                }
                else
                {
                    for (int z = 0; z < Height; z++)
                        path.Add(tiles[z * Width + x]);
                }
            }
            return path.ToArray();
        }


        public TileGrid(int width, int height, float tileSize, Vector3 origin)
        {
            Width = width;
            Height = height;
            TileSize = tileSize;
            Origin = origin;

            centers = new Vector2[width * height];

            float xOff = -((width - 1) * tileSize) * 0.5f;
            float zOff = -((height - 1) * tileSize) * 0.5f;

            int i = 0;

            // ---- ROW-MAJOR: z outer, x inner ----
            for (int z = 0; z < height; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    float wx = origin.x + x * tileSize + xOff;   // centre X
                    float wz = origin.z + z * tileSize + zOff;   // centre Z
                    centers[i++] = new Vector2(wx, wz);
                }
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

        public FieldTile[] Generate(GameObject tilePrefab, Transform parent,
                            float yScale = 0.2f)
        {
            FieldTile[] tiles = new FieldTile[centers.Length];

            for (int i = 0; i < centers.Length; i++)
            {
                // i = z*Width + x  (row-major)
                int z = i / Width;     // row  (GridZ)
                int x = i % Width;     // col  (GridX)

                float y = Terrain.activeTerrain ?
                        Terrain.activeTerrain.SampleHeight(new Vector3(x, 0, z)) :
                        parent.position.y;

                Vector3 pos = new Vector3(
                    centers[i].x,
                    y,
                    centers[i].y);

                var go = Object.Instantiate(tilePrefab, pos, Quaternion.identity, parent);
                go.transform.localScale = new Vector3(TileSize, yScale, TileSize);

                FieldTile tile = go.GetComponent<FieldTile>();
                tile.Init(x, z);

                go.name = $"Tile_{x}_{z}";
                tiles[i] = tile;
                if (i < 10) Debug.Log($"i={i}  =>  ({x},{z})");
            }
            return tiles;
        }
    }
}