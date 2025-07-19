using Harvey.Data.Vehicles;
using Harvey.Farm.Buildings;
using Harvey.Farm.VehicleScripts;

public static class VehicleMapper
{

    public static TModel FromDefinition<TModel>(VehicleDefinition def, GarageBuilding home, string guid)
        where TModel : VehicleModel, new()
    {
        return new TModel
        {
            Id = guid,
            PrefabGuid = def.PrefabGuid,
            DisplayName = def.DisplayName,
            MoveSpeed = def.MoveSpeed,
            Fuel = def.Fuel,
            Durability = def.Durability,
            Price = def.Price,
            Capacity = def.Capacity,
            IsBusy = false,
            Type = def.Type,
            HomeId = home.GetId(),
            CurrentTileIndex = 0,
        };
    }

    public static TractorModel FromSaveData(TractorSaveData d)
    {
        return new TractorModel
        {
            Id = d.Id,
            PrefabGuid = d.PrefabGuid,
            DisplayName = d.DisplayName,
            LoadedPosition = d.Position,
            LoadedRotation = d.Rotation,
            MoveSpeed = d.MoveSpeed,
            Fuel = d.Fuel,
            Durability = d.Durability,
            IsBusy = d.IsBusy,
            Type = d.Type,
            HomeId = d.HomeId,
            CurrentTileIndex = d.CurrentTileIndex,
            AttachedToolId = d.AttachedToolId
        };
    }

    public static HarvesterModel FromSaveData(HarvesterSaveData d)
    {
        return new HarvesterModel
        {
            Id = d.Id,
            PrefabGuid = d.PrefabGuid,
            DisplayName = d.DisplayName,
            LoadedPosition = d.Position,
            LoadedRotation = d.Rotation,
            MoveSpeed = d.MoveSpeed,
            Fuel = d.Fuel,
            Durability = d.Durability,
            IsBusy = d.IsBusy,
            Type = d.Type,
            HomeId = d.HomeId,
            CurrentTileIndex = d.CurrentTileIndex
        };
    }

    public static TractorSaveData ToSaveData(Tractor t)
    {
        if (t.TractorModel== null)
        {
            return new TractorSaveData();
        }

        return new TractorSaveData
        {
            Id = t.TractorModel.Id,
            PrefabGuid = t.TractorModel.PrefabGuid,
            DisplayName = t.TractorModel.DisplayName,
            Position = t.transform.position,
            Rotation = t.transform.rotation,
            MoveSpeed = t.TractorModel.MoveSpeed,
            Fuel = t.TractorModel.Fuel,
            Durability = t.TractorModel.Durability,
            IsBusy = t.TractorModel.IsBusy,
            Type = t.TractorModel.Type,
            HomeId = t.Home.GetId(),
            CurrentTileIndex = t._stats.CurrentTileIndex,
            AttachedToolId = t.TractorModel.AttachedToolId
        };
    }

    public static HarvesterSaveData ToSaveData(CombineHarvester ch)
    {
        if (ch.HarvesterModel == null)
        {
            return new HarvesterSaveData();
        }

        return new HarvesterSaveData
        {
            Id = ch.HarvesterModel.Id,
            PrefabGuid = ch.HarvesterModel.PrefabGuid,
            DisplayName = ch.HarvesterModel.DisplayName,
            Position = ch.transform.position,
            Rotation = ch.transform.rotation,
            Fuel = ch.HarvesterModel.Fuel,
            MoveSpeed = ch.HarvesterModel.MoveSpeed,
            Durability = ch.HarvesterModel.Durability,
            IsBusy = ch.HarvesterModel.IsBusy,
            Type = ch.HarvesterModel.Type,
            HomeId = ch.Home.GetId(),
            CurrentTileIndex = ch._stats.CurrentTileIndex
        };
    }
}