using Harvey.Farm.Buildings;
using Harvey.Data.Implements;
using UnityEngine;

public static class ImplementMappers
{
    public static ImplementModel FromDefinition(ImplementDefinition def, ShedBuilding home, string guid)
    {
        return new ImplementModel
        {
            Id = guid,
            PrefabGuid = def.PrefabGuid,
            DisplayName = def.DisplayName,
            IsReserved = false,
            Type = def.Type,
            Job = def.Job,
            CurrentParentId = home.GetId(),
            HomeId = home.GetId(),
            AnchorIndex = 0,
            Durability = def.Durability,
            Price = def.Price,
        };
    }

    public static ImplementModel FromSaveData(ImplementSaveData d)
    {
        return new ImplementModel
        {
            Id = d.Id,
            PrefabGuid = d.PrefabGuid,
            DisplayName = d.DisplayName,
            LoadedPosition = d.Position,
            LoadedRotation = d.Rotation,
            IsReserved = d.IsReserved,
            Type = d.Type,
            Job = d.Job,
            CurrentParentId = d.CurrentParentId,
            HomeId = d.HomeId,
            AnchorIndex = d.AnchorIndex,
            Durability = d.Durability,
            Price = d.Price
        };
    }

    public static ImplementSaveData ToSaveData(ImplementBehaviour beh)
    {
        if (beh.Model == null)
        {
            return new ImplementSaveData();
        }

        return new ImplementSaveData
        {
            Id = beh.Model.Id,
            PrefabGuid = beh.Model.PrefabGuid,
            DisplayName = beh.Model.DisplayName,
            Position = beh.transform.position,
            Rotation = beh.transform.rotation,
            IsReserved = beh.Model.IsReserved,
            Type = beh.Model.Type,
            Job = beh.Model.Job,
            CurrentParentId = beh.Model.CurrentParentId,
            HomeId = beh.Home != null ? beh.Home.GetId() : string.Empty,
            AnchorIndex = beh.GetAnchorIndex(),
            Durability = beh.Model.Durability,
            Price = beh.Model.Price
        };
    }
}
