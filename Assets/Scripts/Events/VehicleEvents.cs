using System;
using Harvey.Farm.VehicleScripts;

namespace Harvey.Farm.Events
{
    public static class VehicleEvents
    {
        public static event Action<Vehicle, bool> OnVehicleBusyChanged;

        public static void VehicleBusyChanged(Vehicle v, bool isBusy) => OnVehicleBusyChanged?.Invoke(v, isBusy);
    }
}
