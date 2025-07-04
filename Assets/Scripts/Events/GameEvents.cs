using System;
using Harvey.Farm.Buildings;
using Harvey.Farm.Crops;
using Harvey.Farm.Fields;
using Harvey.Farm.JobScripts;
using Harvey.Farm.VehicleScripts;

namespace Harvey.Farm.Events
{
    public static class GameEvents
    {

        // *------------------- Vehicles -------------------*
        public static event Action<Vehicle, bool> OnVehicleBusyChanged;
        public static void VehicleBusyChanged(Vehicle v, bool isBusy) => OnVehicleBusyChanged?.Invoke(v, isBusy);

        // *------------------- Fields -------------------*
        public static event Action<FieldController> OnFieldSelected;
        public static event Action<FieldTile> OnTilePlowed;
        public static event Action<FieldTile, CropDefinition> OnTileSeeded;
        public static event Action<FieldTile> OnTileHarvested;
        public static event Action<FieldController> OnFieldCompleted;
        public static event Action<FieldController> OnFieldGrown;
        public static event Action<FieldController> OnFieldHarvested;


        public static void FieldSelected(FieldController f) => OnFieldSelected?.Invoke(f);
        public static void TilePlowed(FieldTile tile) => OnTilePlowed?.Invoke(tile);
        public static void TileSeeded(FieldTile tile, CropDefinition crop) => OnTileSeeded?.Invoke(tile, crop);
        public static void TileHarvested(FieldTile tile) => OnTileHarvested?.Invoke(tile);
        public static void FieldCompleted(FieldController field) => OnFieldCompleted?.Invoke(field);
        public static void FieldGrown(FieldController field) => OnFieldGrown?.Invoke(field);
        public static void FieldHarvested(FieldController field) => OnFieldHarvested?.Invoke(field);

        // *------------------- Jobs -------------------*
        public static event Action<Vehicle, FieldJob> OnJobStarted;
        public static void JobStarted(Vehicle v, FieldJob fieldJob) => OnJobStarted?.Invoke(v, fieldJob);

        // *------------------- Buildings -------------------*
        public static event Action OnShedInventoryChanged;
        public static void ShedInventoryChanged() => OnShedInventoryChanged?.Invoke();

        // *------------------- Debug -------------------*
        public static event Action<bool> OnDebugModeToggled;
        public static void DebugModeToggled(bool enabled) => OnDebugModeToggled?.Invoke(enabled);

        // *------------------- UI -------------------*
        public static event Action<FieldJob, Vehicle> OnJobButtonPressed;
        public static event Action OnCloseUI;
        public static void JobButtonPressed(FieldJob j, Vehicle v) => OnJobButtonPressed?.Invoke(j, v);
        public static void CloseUI() => OnCloseUI?.Invoke();

    }
}
