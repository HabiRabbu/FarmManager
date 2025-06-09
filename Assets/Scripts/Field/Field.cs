using UnityEngine;

namespace Harvey.Farm.FieldScripts
{
    public class Field : MonoBehaviour
    {
        [SerializeField] GameObject tilePrefab;
        [SerializeField] int width, height;
        [SerializeField] float tileSize = 1f;

        private TileGrid grid;
        private FieldTile[] tiles;
        private enum State { Idle, Plowing, Plowed }
        [SerializeField] State currentState = State.Idle;

        void Awake()
        {
            FieldManager.Instance.RegisterField(this);
            grid = new TileGrid(width, height, tileSize, transform.position);
            tiles = grid.Generate(tilePrefab, transform);
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
            tile.Plow();
            if (System.Array.TrueForAll(tiles, t => t.IsPlowed))
                currentState = State.Plowed;
        }
    }
}
