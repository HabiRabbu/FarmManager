using System;
using Harvey.Data.Coffee;
using Harvey.Farm.Buildings;
using Harvey.Farm.Fields;
using Harvey.Farm.Jobs;
using Harvey.Farm.VehicleScripts;
using Harvey.Farm.Workers;

namespace Harvey.Farm.Events
{
    public static class GameEvents
    {

        // *------------------- General -------------------*
        public static event Action OnStopAllTweens;
        public static void StopAllTweens() => OnStopAllTweens?.Invoke();

        // *------------------- Game Management -------------------*
        public static event Action OnGameReady;
        public static event Action OnGameIsPlaying;
        public static event Action<float> OnPreloadProgress;
        public static event Action OnPreloadComplete;
        public static event Action<string> OnAssetPreloaded;

        public static void GameReady() => OnGameReady?.Invoke();
        public static void GameIsPlaying() => OnGameIsPlaying?.Invoke();
        public static void PreloadProgress(float progress) => OnPreloadProgress?.Invoke(progress);
        public static void PreloadComplete() => OnPreloadComplete?.Invoke();
        public static void AssetPreloaded(string assetId) => OnAssetPreloaded?.Invoke(assetId);

        // *------------------- JobAgents -------------------*
        public static event Action<Vehicle, bool> OnVehicleBusyChanged;
        public static event Action<Worker, bool> OnWorkerBusyChanged;
        public static void VehicleBusyChanged(Vehicle v, bool isBusy) => OnVehicleBusyChanged?.Invoke(v, isBusy);
        public static void WorkerBusyChanged(Worker w, bool isBusy) => OnWorkerBusyChanged?.Invoke(w, isBusy);

        // *------------------- Fields -------------------*
        public static event Action<FieldTile> OnTilePlowed;
        public static event Action<FieldTile, CoffeeCropData> OnTileSeeded;
        public static event Action<FieldTile> OnTileHarvested;
        public static event Action<FieldController> OnFieldCompleted;
        public static event Action<FieldController> OnFieldGrown;
        public static event Action<FieldController> OnFieldHarvested;

        public static void TilePlowed(FieldTile tile) => OnTilePlowed?.Invoke(tile);
        public static void TileSeeded(FieldTile tile, CoffeeCropData crop) => OnTileSeeded?.Invoke(tile, crop);
        public static void TileHarvested(FieldTile tile) => OnTileHarvested?.Invoke(tile);
        public static void FieldCompleted(FieldController field) => OnFieldCompleted?.Invoke(field);
        public static void FieldGrown(FieldController field) => OnFieldGrown?.Invoke(field);
        public static void FieldHarvested(FieldController field) => OnFieldHarvested?.Invoke(field);

        // *------------------- Jobs -------------------*
        public static event Action<IJobAgent, FieldJob> OnJobStarted;
        public static void JobStarted(IJobAgent jobAgent, FieldJob fieldJob) => OnJobStarted?.Invoke(jobAgent, fieldJob);

        // *------------------- Buildings -------------------*
        public static event Action OnBuildingStatsChanged;
        public static void BuildingStatsChanged() => OnBuildingStatsChanged?.Invoke();

        // *------------------- Debug -------------------*
        public static event Action<bool> OnDebugModeToggled;
        public static void DebugModeToggled(bool enabled) => OnDebugModeToggled?.Invoke(enabled);

        // *------------------- UI -------------------*
        public static event Action<FieldJob, Vehicle> OnJobButtonPressed;
        public static event Action OnEscapePressed;
        public static event Action OnOptionsMenuOpened;
        public static event Action OnOptionsMenuClosed;
        
        public static void JobButtonPressed(FieldJob j, Vehicle v) => OnJobButtonPressed?.Invoke(j, v);
        public static void EscapePressed() => OnEscapePressed?.Invoke();
        public static void OptionsMenuOpened() => OnOptionsMenuOpened?.Invoke();
        public static void OptionsMenuClosed() => OnOptionsMenuClosed?.Invoke();

        // *------------------- Radial Menu -------------------*
        public static event Action<FieldController> OnRadialFieldInfoOpened;
        public static event Action<FieldController> OnRadialFieldTractorOpened;
        public static event Action<FieldController> OnRadialFieldWorkersOpened;
        public static void RadialFieldInfoOpened(FieldController field) => OnRadialFieldInfoOpened?.Invoke(field);
        public static void RadialFieldTractorOpened(FieldController field) => OnRadialFieldTractorOpened?.Invoke(field);
        public static void RadialFieldWorkersOpened(FieldController field) => OnRadialFieldWorkersOpened?.Invoke(field);
        public static event Action<Building> OnRadialBuildingInfoOpened;
        public static void RadialBuildingInfoOpened(Building building) => OnRadialBuildingInfoOpened?.Invoke(building);

    }
}
