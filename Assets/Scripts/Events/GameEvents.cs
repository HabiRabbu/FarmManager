using System;
using Harvey.Farm.FieldScripts;
using Harvey.Farm.VehicleScripts;

namespace Harvey.Farm.Events
{
    public static class GameEvents
    {

        // *------------------- Vehicles -------------------*
        public static event Action<Vehicle, bool> OnVehicleBusyChanged;
        public static void VehicleBusyChanged(Vehicle v, bool isBusy) => OnVehicleBusyChanged?.Invoke(v, isBusy);

        // *------------------- Fields -------------------*
        public static event Action<FieldTile> OnTilePloughed;
        public static event Action<Field> OnFieldCompleted;

        public static void TilePloughed(FieldTile tile) => OnTilePloughed?.Invoke(tile);
        public static void FieldCompleted(Field field) => OnFieldCompleted?.Invoke(field);

        // *------------------- Jobs -------------------*
        public static event Action<Vehicle, Field, JobType> OnJobStarted;
        public static void JobStarted(Vehicle v, Field f, JobType j) => OnJobStarted?.Invoke(v, f, j);
        
        // *------------------- Debug -------------------*
        public static event Action<bool> OnDebugModeToggled;
        public static void DebugModeToggled(bool enabled) => OnDebugModeToggled?.Invoke(enabled);

        // *------------------- UI -------------------*
        public static event Action<Field, Vehicle, JobType> OnJobButtonPressed;
        public static void JobButtonPressed(Field f, Vehicle v, JobType t) => OnJobButtonPressed?.Invoke(f, v, t);

    }
}
