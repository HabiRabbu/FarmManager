using System;
using Harvey.Farm.FieldScripts;

namespace Harvey.Farm.Events
{
    public static class WorldEvents
    {
        public static event Action<FieldTile> OnTilePloughed;
        public static event Action<Field> OnFieldCompleted;


        public static void TilePloughed(FieldTile tile) => OnTilePloughed?.Invoke(tile);
        public static void FieldCompleted(Field field) => OnFieldCompleted?.Invoke(field);
    }
}
