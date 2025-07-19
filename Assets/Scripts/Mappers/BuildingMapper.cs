using Harvey.Data.Buildings;
using UnityEngine;

public static class BuildingMapper
{
    public static TModel FromDefinition<TModel>(BuildingDefinition def, string guid)
        where TModel : BuildingModel, new()
    {
        return new TModel
        {
            Id = guid,
            PrefabGuid = def.PrefabGuid,
            DisplayName = def.DisplayName,
            Type = def.Type,
            Capacity = def.Capacity,
            Price = (int)def.Price
        };
    }

    public static ShedModel FromSaveData(ShedSaveData data)
    {
        return new ShedModel
        {
            Id = data.Id,
            PrefabGuid = data.PrefabGuid,
            DisplayName = data.DisplayName,
            LoadedPosition = data.Position,
            LoadedRotation = data.Rotation,
            Type = data.Type,
            Capacity = data.Capacity,
            Price = data.Price
        };
    }

    public static GarageModel FromSaveData(GarageSaveData data)
    {
        return new GarageModel
        {
            Id = data.Id,
            PrefabGuid = data.PrefabGuid,
            DisplayName = data.DisplayName,
            LoadedPosition = data.Position,
            LoadedRotation = data.Rotation,
            Type = data.Type,
            Capacity = data.Capacity,
            Price = data.Price
        };
    }

    public static HouseModel FromSaveData(HouseSaveData data)
    {
        return new HouseModel
        {
            Id = data.Id,
            PrefabGuid = data.PrefabGuid,
            DisplayName = data.DisplayName,
            LoadedPosition = data.Position,
            LoadedRotation = data.Rotation,
            Type = data.Type,
            Capacity = data.Capacity,
            Price = data.Price,
            SpawnRadius = data.SpawnRadius
        };
    }

    public static ShedSaveData ToSaveData(ShedModel model, Vector3 position, Quaternion rotation)
    {
        if (model == null)
        {
            return new ShedSaveData();
        }

        return new ShedSaveData
        {
            Id = model.Id,
            PrefabGuid = model.PrefabGuid,
            DisplayName = model.DisplayName,
            Position = position,
            Rotation = rotation,
            Type = model.Type,
            Capacity = model.Capacity,
            Price = model.Price
        };
    }

    public static GarageSaveData ToSaveData(GarageModel model, Vector3 position, Quaternion rotation)
    {
        if (model == null)
        {
            return new GarageSaveData();
        }

        return new GarageSaveData
        {
            Id = model.Id,
            PrefabGuid = model.PrefabGuid,
            DisplayName = model.DisplayName,
            Position = position,
            Rotation = rotation,
            Type = model.Type,
            Capacity = model.Capacity,
            Price = model.Price
        };
    }

    public static HouseSaveData ToSaveData(HouseModel model, Vector3 position, Quaternion rotation)
    {
        if (model == null)
        {
            return new HouseSaveData();
        }

        return new HouseSaveData
        {
            Id = model.Id,
            PrefabGuid = model.PrefabGuid,
            DisplayName = model.DisplayName,
            Position = position,
            Rotation = rotation,
            Type = model.Type,
            Capacity = model.Capacity,
            Price = model.Price,
            SpawnRadius = model.SpawnRadius
        };
    }
}