using UnityEngine;

namespace Harvey.Farm.FieldScripts
{
    public class Field : MonoBehaviour
    {
        private enum FieldState
        {
            Idle,
            Plowing,
            Plowed
        }

        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private int width, height;
        [SerializeField] private float tileSize = 1f;
        private FieldTile[,] tiles;
        [SerializeField] private FieldState currentState = FieldState.Idle;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            GenerateGrid();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void GenerateGrid()
        {
            tiles = new FieldTile[width, height];
            float xOffset = -(width * tileSize) / 2f;
            float zOffset = -(height * tileSize) / 2f;


            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    Vector3 tilePosition = new Vector3(
                        transform.position.x + x * tileSize + xOffset,
                        transform.position.y,
                        transform.position.z + z * tileSize + zOffset
                    );

                    GameObject tileObject = Instantiate(tilePrefab, tilePosition, Quaternion.identity);

                    tileObject.transform.SetParent(this.transform, true);

                    FieldTile tile = tileObject.GetComponent<FieldTile>();
                    tiles[x, z] = tile;

                    tile.SetGridPosition(x, z);
                    tileObject.name = $"Tile_x{x}_z{z}";
                }
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawWireCube(transform.position, new Vector3(width, 1, height));
        }

    }
}
