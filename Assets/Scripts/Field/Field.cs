using UnityEngine;
using Harvey.Farm.VehicleScripts;

namespace Harvey.Farm.FieldScripts
{
    public class Field : MonoBehaviour
    {
        [SerializeField] GameObject tilePrefab;
        [SerializeField] int width, height;
        private float tileSize = 1f;

        private TileGrid grid;
        private FieldTile[] tiles;
        private FieldTile[] serpentinePath;
        public FieldTile[] GetSerpentineTiles() => serpentinePath;
        private enum State { Idle, Plowing, Plowed }
        [SerializeField] State currentState = State.Idle;


        //DEBUG - GIZMOS
        private Vector3[] waypointWorlds;
        //---------------

        void Start()
        {
            tileSize = FieldManager.Instance.tileSize;

            FieldManager.Instance.RegisterField(this);
            grid = new TileGrid(width, height, tileSize, transform.position);
            tiles = grid.Generate(tilePrefab, transform);

            GenerateSerpentinePath();

            // Cache world positions for gizmos
            waypointWorlds = new Vector3[serpentinePath.Length];
            for (int i = 0; i < serpentinePath.Length; i++)
                waypointWorlds[i] = serpentinePath[i].WorldPosition + Vector3.up * 0.5f;
        }

        private void GenerateSerpentinePath()
        {
            if (width >= height)
            {
                serpentinePath = grid.BuildSerpentineRows(tiles);
            }
            else
            {
                serpentinePath = grid.BuildSerpentineColumns(tiles);
            }
        }

        void OnDestroy()
        {
            FieldManager.Instance.UnregisterField(this);
        }

        public bool ContainsPoint(Vector3 worldPos) => grid.Contains(worldPos);

        public void HandleTileClick(Vector3 worldPos)
        {
            if (!grid.Contains(worldPos)) return;

            int idx = grid.IndexOfNearest(worldPos);
            var tile = tiles[idx];

            Tractor tractor = VehicleManager.Instance.GetAvailableVehicle<Tractor>();
            if (tractor == null)
            {
                Debug.Log("No available tractors.");
                return;
            }

            tractor.StartTask(tile);

            if (System.Array.TrueForAll(tiles, t => t.IsPlowed))
                currentState = State.Plowed;
        }




#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (width <= 0 || height <= 0 || tileSize <= 0f)
                return;

            Gizmos.color = Color.green;

            float xOffset = -((width - 1) * tileSize) / 2f;
            float zOffset = -((height - 1) * tileSize) / 2f;
            Vector3 origin = transform.position + new Vector3(xOffset, 0f, zOffset);

            Vector3 topRight = origin + new Vector3(width * tileSize, 0f, 0f);
            Vector3 bottomLeft = origin + new Vector3(0f, 0f, height * tileSize);
            Vector3 bottomRight = origin + new Vector3(width * tileSize, 0f, height * tileSize);
            Gizmos.DrawLine(origin, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, origin);

            for (int x = 1; x < width; x++)
            {
                Vector3 start = origin + new Vector3(x * tileSize, 0f, 0f);
                Vector3 end = start + new Vector3(0f, 0f, height * tileSize);
                Gizmos.DrawLine(start, end);
            }
            for (int z = 1; z < height; z++)
            {
                Vector3 start = origin + new Vector3(0f, 0f, z * tileSize);
                Vector3 end = start + new Vector3(width * tileSize, 0f, 0f);
                Gizmos.DrawLine(start, end);
            }
        }

        void OnDrawGizmosSelected()
        {
            if (waypointWorlds == null || waypointWorlds.Length == 0)
                return;

            Gizmos.color = Color.yellow;
            foreach (var wp in waypointWorlds)
                Gizmos.DrawSphere(wp, 0.1f);

            Gizmos.color = Color.cyan;
            for (int i = 0; i < waypointWorlds.Length - 1; i++)
                Gizmos.DrawLine(waypointWorlds[i], waypointWorlds[i + 1]);
        }
#endif

    }
}
