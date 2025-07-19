using System.Threading.Tasks;
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
            width = definition?.Width ?? 0;
            height = definition?.Height ?? 0;
            tileSize = definition?.TileSize ?? 1f;
            tilePrefabGuid = definition?.TilePrefabGuid ?? "basic-field-tile"; //TODO: Stop hardcoding this

            buildDataPresent = definition != null;
        }

        public async Task BuildFromModelAsync(FieldModel model)
        {
            if (model == null) return;

            definition = null;
            width = model.Width;
            height = model.Height;
            tileSize = model.TileSize;
            tilePrefabGuid = "basic-field-tile"; //TODO: Stop hardcoding this

            buildDataPresent = true;

            await BuildAsync();
        }

        public async Task BuildAsync()
        {
            if (!buildDataPresent) return;

            if (Tiles != null)
                foreach (var t in Tiles)
                    FieldTileFactory.Instance.Despawn(tilePrefabGuid ?? "basic-field-tile", t.gameObject);

            Grid = new TileGrid(width, height, tileSize, transform.position);
            Tiles = await Grid.GenerateAsync(tilePrefabGuid, transform);

            Debug.Log($"✅ FieldBuilder: Built field with {Tiles?.Length ?? 0} tiles");
        }


        public bool ContainsPoint(Vector3 worldPos)
        {
            return Grid != null && Grid.Contains(worldPos);
        }
    }
}

