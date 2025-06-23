using UnityEngine;
using Harvey.Farm.VehicleScripts;
using Harvey.Farm.Events;
using Harvey.Farm.UI;
using System.Collections.Generic;
using Harvey.Farm.Crops;
using System.Collections;

namespace Harvey.Farm.FieldScripts
{
    public class Field : MonoBehaviour
    {
        private int tilesCompletedSoFar = 0;
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
        public enum State
        {
            Idle,

            // Ploughing
            Plowing,
            Plowed,

            // Seeding
            Seeding,
            Seeded,

            //Growing
            Growing,
            ReadyToHarvest,

            // Harvesting
            Harvesting,
            Harvested
        }
        public State Current => currentState;
        public bool Is(State s) => currentState == s;
        public void SetState(State s)
        {
            currentState = s;
        }
        // --------------------------------------------------------------------------------

        [SerializeField] public CropDefinition currentCrop;

        //--DEBUG - GIZMOS--
        private Vector3[] waypointWorlds;
        //------------------

        public float TilesCompletedFraction => (float)tilesCompletedSoFar / tiles.Length;

        public bool ContainsPoint(Vector3 worldPos) => grid.Contains(worldPos);

        void OnEnable()
        {
            GameEvents.OnTilePlowed += HandleTilePlowed;
            GameEvents.OnTileSeeded += HandleTileSeeded;
        }
        void OnDisable()
        {
            GameEvents.OnTilePlowed -= HandleTilePlowed;
            GameEvents.OnTileSeeded -= HandleTileSeeded;
        }

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

        private void HandleTilePlowed(FieldTile t)
        {
            HandleTileCompleted(t, null);
        }
        private void HandleTileSeeded(FieldTile t, CropDefinition crop)
        {
            HandleTileCompleted(t, crop);
        }
        void HandleTileCompleted(FieldTile t, CropDefinition crop = null)
        {
            if (t.transform.parent != transform) return;
            tilesCompletedSoFar++;

            if (tilesCompletedSoFar >= tiles.Length && currentState == State.Plowing)
            {
                SetState(State.Plowed);
                GameEvents.FieldCompleted(this);
            }
            if (tilesCompletedSoFar >= tiles.Length && currentState == State.Seeding)
            {
                SetState(State.Seeded);
                GameEvents.FieldCompleted(this);
                StartGrowing();
            }
            if (tilesCompletedSoFar >= tiles.Length && currentState == State.Harvesting)
            {
                SetState(State.Harvested);
                GameEvents.FieldCompleted(this);
            }
        }

        // Does this field need <type> work?
        public bool Needs(JobType type) => type switch
        {
            JobType.Plow => !Is(State.Plowed),
            JobType.Seed => Is(State.Plowed) && !Is(State.Seeded),
            JobType.Harvest => Is(State.ReadyToHarvest) && !Is(State.Harvested),
            _ => false
        };

        // Call when a tractor is about to start <type>.
        public void Begin(JobType type, CropDefinition crop = null)
        {
            currentState = type switch
            {
                JobType.Plow => State.Plowing,
                JobType.Seed => State.Seeding,
                JobType.Harvest => State.Harvesting,
                _ => currentState
            };

            if (type == JobType.Seed && crop != null)
                currentCrop = crop;

            tilesCompletedSoFar = 0;
        }

        private void StartGrowing()
        {
            if (Is(State.Seeded) && currentCrop != null)
            {
                SetState(State.Growing);
                StartCoroutine(GrowRoutine());
            }
            else
            {
                Debug.LogWarning($"Field {fieldName} cannot grow: not seeded or crop not set.");
            }
        }

        IEnumerator GrowRoutine()
        {
            SetState(State.Growing);

            float step = currentCrop.growSeconds / 2f;

            yield return new WaitForSeconds(step);
            foreach (var t in tiles) t.SetStage(1);

            yield return new WaitForSeconds(step);
            foreach (var t in tiles) t.SetStage(2);

            SetState(State.ReadyToHarvest);
            GameEvents.FieldGrown(this);
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
