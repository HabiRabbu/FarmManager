using UnityEngine;

namespace Harvey.Farm.Fields
{
    public class FieldBuilder : MonoBehaviour
    {
        FieldDefinition definition;
        public FieldTile[] Tiles { get; private set; }
        public TileGrid Grid { get; private set; }

        void Awake()
        {
            if (definition == null)
            {
                var controller = GetComponent<FieldController>();
                if (controller != null)
                    definition = controller.definition;
            }
        }

        public void Build()
        {
            if (definition == null) return;

            Grid = new TileGrid(definition.width, definition.height, definition.tileSize, transform.position);
            Tiles = Grid.Generate(definition.tilePrefab, transform);
        }

        public bool ContainsPoint(Vector3 worldPos)
        {
            return Grid != null && Grid.Contains(worldPos);
        }
    }
}

