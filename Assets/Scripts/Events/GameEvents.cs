using System;
using Harvey.Farm.Crops;
using Harvey.Farm.FieldScripts;
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
        public static event Action<Field> OnFieldSelected;
        public static event Action<FieldTile> OnTilePlowed;
        public static event Action<FieldTile, CropDefinition> OnTileSeeded;
        public static event Action<FieldTile> OnTileHarvested;
        public static event Action<Field> OnFieldCompleted;
        public static event Action<Field> OnFieldGrown;
        public static event Action<Field> OnFieldHarvested;


        public static void FieldSelected(Field f) => OnFieldSelected?.Invoke(f);
        public static void TilePlowed(FieldTile tile) => OnTilePlowed?.Invoke(tile);
        public static void TileSeeded(FieldTile tile, CropDefinition crop) => OnTileSeeded?.Invoke(tile, crop);
        public static void TileHarvested(FieldTile tile) => OnTileHarvested?.Invoke(tile);
        public static void FieldCompleted(Field field) => OnFieldCompleted?.Invoke(field);
        public static void FieldGrown(Field field) => OnFieldGrown?.Invoke(field);
        public static void FieldHarvested(Field field) => OnFieldHarvested?.Invoke(field);

        // *------------------- Jobs -------------------*
        public static event Action<Vehicle, Field, JobType> OnJobStarted;
        public static void JobStarted(Vehicle v, Field f, JobType j) => OnJobStarted?.Invoke(v, f, j);

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
