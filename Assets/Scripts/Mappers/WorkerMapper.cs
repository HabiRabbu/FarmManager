using Harvey.Data.Workers;
using Harvey.Farm.Buildings;
using Harvey.Farm.Workers;

public static class WorkerMapper
{
    public static WorkerModel FromDefinition(WorkerDefinition def, HouseBuilding home, string guid)
    {
        return new WorkerModel
        {
            Id = guid,
            PrefabGuid = def.PrefabGuid,
            PortraitGuid = def.PortraitGuid,
            DisplayName = def.DisplayName,
            ActiveInScene = false, // Workers start inside the house
            IsBusy = false,
            HomeId = home.GetId(),
            CurrentTileIndex = 0,
            WalkSpeed = def.WalkSpeed,
            plowSeconds = def.plowSeconds,
            seedSeconds = def.seedSeconds,
            harvestSeconds = def.harvestSeconds
        };
    }

    public static WorkerModel FromSaveData(WorkerSaveData d)
    {
        return new WorkerModel
        {
            Id = d.Id,
            PrefabGuid = d.PrefabGuid,
            PortraitGuid = d.PortraitGuid,
            DisplayName = d.DisplayName,
            LoadedPosition = d.Position,
            LoadedRotation = d.Rotation,
            ActiveInScene = d.ActiveInScene,
            IsBusy = d.IsBusy,
            HomeId = d.HomeId,
            CurrentTileIndex = d.CurrentTileIndex,
            WalkSpeed = d.WalkSpeed,
            plowSeconds = d.plowSeconds,
            seedSeconds = d.seedSeconds,
            harvestSeconds = d.harvestSeconds
        };
    }

    public static WorkerSaveData ToSaveData(Worker worker)
    {
        if (worker == null)
        {
            return new WorkerSaveData();
        }

        return new WorkerSaveData
        {
            Id = worker.GetId(),
            PrefabGuid = worker.Model.PrefabGuid,
            PortraitGuid = worker.Model.PortraitGuid,
            DisplayName = worker.Model.DisplayName,
            Position = worker.transform.position,
            Rotation = worker.transform.rotation,
            ActiveInScene = worker.gameObject.activeInHierarchy,
            IsBusy = worker.IsBusy,
            HomeId = worker.Home.GetId(),
            CurrentTileIndex = worker.Model.CurrentTileIndex,
            WalkSpeed = worker.Model.WalkSpeed,
            plowSeconds = worker.Model.plowSeconds,
            seedSeconds = worker.Model.seedSeconds,
            harvestSeconds = worker.Model.harvestSeconds
        };
    }
}
