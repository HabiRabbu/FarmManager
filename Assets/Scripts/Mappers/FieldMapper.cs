using Harvey.Data.Fields;
using UnityEngine;

public static class FieldMapper
{
    public static FieldModel FromDefinition(FieldDefinition def, string guid, Vector3 pos)
    {
        return new FieldModel
        {
            Id = guid,
            DisplayName = def.FieldName,
            PrefabGuid = def.PrefabGuid,
            Position = pos,
            Width = def.Width,
            Height = def.Height,
            TileSize = def.TileSize,
            CurrentState = 0,
            CurrentCropId = string.Empty,
            TileFlags = new string('.', def.Width * def.Height)
        };
    }

    public static FieldSaveData ToSaveData(FieldModel m)
    {
        return new FieldSaveData
        {
            Id = m.Id,
            DisplayName = m.DisplayName,
            PrefabGuid = m.PrefabGuid,
            Position = m.Position,
            Width = m.Width,
            Height = m.Height,
            TileSize = m.TileSize,
            CurrentState = m.CurrentState,
            CurrentCropId = m.CurrentCropId,
            TileFlags = m.TileFlags
        };
    }

    public static FieldModel FromSaveData(FieldSaveData d)
    {
        return new FieldModel
        {
            Id = d.Id,
            DisplayName = d.DisplayName,
            PrefabGuid = d.PrefabGuid,
            Position = d.Position,
            Width = d.Width,
            Height = d.Height,
            TileSize = d.TileSize,
            CurrentState = d.CurrentState,
            CurrentCropId = d.CurrentCropId,
            TileFlags = d.TileFlags
        };
    }
}
