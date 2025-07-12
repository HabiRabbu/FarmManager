using Harvey.Data.Fields;
using Harvey.Farm.Factory;
using UnityEngine;

namespace Harvey.Farm.Fields
{
    public class FieldBuilder : MonoBehaviour
    {
        FieldDefinition definition;
        public FieldTile[] Tiles { get; private set; }
        public TileGrid Grid { get; private set; }

        private bool buildDataPresent = false;

        private int width;
        private int height;
        private float tileSize;
        private string tilePrefabGuid;

        void Awake()
        {
            InitializeDefinition();
            InitializeBuildData();
        }

        private void InitializeDefinition()
        {
            if (definition == null)
            {
                var controller = GetComponent<FieldController>();
                if (controller != null)
                    definition = controller.definition;
            }
        }

        private void InitializeBuildData()
        {
            width = definition?.width ?? 0;
            height = definition?.height ?? 0;
            tileSize = definition?.tileSize ?? 1f;
            tilePrefabGuid = definition?.tilePrefabGuid ?? "basic-field-tile"; //TODO: Stop hardcoding this

            buildDataPresent = definition != null;
        }

        public void BuildFromData(FieldSaveData data)
        {
            if (data == null) return;

            definition = null;
            width = data.Width;
            height = data.Height;
            tileSize = data.TileSize;
            tilePrefabGuid = data.TilePrefabGuid;

            buildDataPresent = true;

            Build();
        }

        public void Build()
        {
            if (!buildDataPresent) return;

            if (Tiles != null)
                foreach (var t in Tiles)
                    FieldTileFactory.Instance.Despawn(tilePrefabGuid?? "basic-field-tile", t.gameObject);

            Grid = new TileGrid(width, height, tileSize, transform.position);
            Tiles = Grid.Generate(tilePrefabGuid, transform);
        }


        public bool ContainsPoint(Vector3 worldPos)
        {
            return Grid != null && Grid.Contains(worldPos);
        }
    }
}

