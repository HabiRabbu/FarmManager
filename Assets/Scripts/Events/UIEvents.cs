using System;
using Harvey.Farm.FieldScripts;
using Harvey.Farm.VehicleScripts;

namespace Harvey.Farm.Events
{ 
    public static class UIEvents
    {
        public static event Action<Field, Vehicle> OnJobButtonPressed;
        public static void JobButtonPressed(Field f, Vehicle v) => OnJobButtonPressed?.Invoke(f, v);
    }
}
