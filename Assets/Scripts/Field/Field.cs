using UnityEngine;
using Harvey.Farm.VehicleScripts;
using Harvey.Farm.Events;
using Harvey.Farm.UI;
using System.Collections.Generic;

namespace Harvey.Farm.FieldScripts
{
    public class Field : MonoBehaviour
    {
        private int plowedSoFar = 0;
        [SerializeField] public string fieldName = "fieldNameNotSet";

        // Tile + Grid Stuff
        [SerializeField] GameObject tilePrefab;
        [SerializeField] int width, height;
        private float tileSize = 1f;
        private TileGrid grid;
        private FieldTile[] tiles;
        private FieldTile[] serpentinePath;
        public FieldTile[] GetSerpentineTiles() => serpentinePath;

        //----------------------------- STATES ---------------------------------------
        [SerializeField] State currentState = State.Idle;
        private enum State
        {
            Idle,

            // Ploughing
            Plowing,
            Plowed,

            // Seeding
            Seeding,
            Seeded,

            // Harvesting
            Harvesting,
            Harvested
        }
        public bool IsIdle => currentState == State.Idle;
        public bool IsPlowing => currentState == State.Plowing;
        public bool IsPlowed => currentState == State.Plowed;

        public bool IsSeeding => currentState == State.Seeding;
        public bool IsSeeded => currentState == State.Seeded;

        public bool IsHarvesting => currentState == State.Harvesting;
        public bool IsHarvested => currentState == State.Harvested;
        // --------------------------------------------------------------------------------

        [Header("UI")]
        [SerializeField] FieldJobPanel panel;

        int remainingTiles;


        //--DEBUG - GIZMOS--
        private Vector3[] waypointWorlds;
        //------------------

        public float TilesPlowedFraction => (float)plowedSoFar / tiles.Length;

        public bool ContainsPoint(Vector3 worldPos) => grid.Contains(worldPos);

        void OnEnable() => GameEvents.OnTilePloughed += HandleTilePlowed;
        void OnDisable() => GameEvents.OnTilePloughed -= HandleTilePlowed;

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

            //Setup Field UI
            panel.Init(this);
            UIManager.Instance.Register(panel);

            remainingTiles = tiles.Length;
        }

        public void HandleTileClick(Vector3 worldPos)
        {
            if (!grid.Contains(worldPos)) return;
            panel.Show(worldPos);
        }

        // Does this field need <type> work?
        public bool Needs(JobType type) => type switch
        {
            JobType.Plow => !IsPlowed,
            JobType.Seed => IsPlowed && !IsSeeded,
            JobType.Harvest => IsSeeded && !IsHarvested,
            _ => false
        };

        // Call when a tractor is about to start <type>.
        public void Begin(JobType type)
        {
            currentState = type switch
            {
                JobType.Plow => State.Plowing,
                JobType.Seed => State.Seeding,
                JobType.Harvest => State.Harvesting,
                _ => currentState
            };

            plowedSoFar = 0;
            remainingTiles = tiles.Length;
        }

        void HandleTilePlowed(FieldTile t)
        {
            if (t.transform.parent != transform) return;

            plowedSoFar++;
            if (plowedSoFar >= tiles.Length && currentState == State.Plowing)
            {
                currentState = State.Plowed;
                GameEvents.FieldCompleted(this);
            }
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

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (width <= 0 || height <= 0 || tileSize <= 0f) return;

            Gizmos.color = Color.green;

            Vector3 centre = transform.position;
            float halfW = width * tileSize * 0.5f;
            float halfH = height * tileSize * 0.5f;
            float y = centre.y;

            // ----- outer frame -----
            Vector3 bl = new Vector3(centre.x - halfW, y, centre.z - halfH);
            Vector3 br = new Vector3(centre.x + halfW, y, centre.z - halfH);
            Vector3 tl = new Vector3(centre.x - halfW, y, centre.z + halfH);
            Vector3 tr = new Vector3(centre.x + halfW, y, centre.z + halfH);

            Gizmos.DrawLine(tl, tr);
            Gizmos.DrawLine(tr, br);
            Gizmos.DrawLine(br, bl);
            Gizmos.DrawLine(bl, tl);

            // ----- internal grid (vertical) -----
            for (int x = 1; x < width; x++)
            {
                float xPos = centre.x - halfW + x * tileSize;
                Gizmos.DrawLine(
                    new Vector3(xPos, y, centre.z - halfH),
                    new Vector3(xPos, y, centre.z + halfH));
            }

            // ----- internal grid (horizontal) -----
            for (int z = 1; z < height; z++)
            {
                float zPos = centre.z - halfH + z * tileSize;
                Gizmos.DrawLine(
                    new Vector3(centre.x - halfW, y, zPos),
                    new Vector3(centre.x + halfW, y, zPos));
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
