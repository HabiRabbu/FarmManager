using System;
using Harvey.Farm.FieldScripts;
using Harvey.Farm.VehicleScripts;

namespace Harvey.Farm.Events
{ 
    public static class UIEvents
    {
        public static event Action<Field, Vehicle, JobType> OnJobButtonPressed;
        public static void JobButtonPressed(Field f, Vehicle v, JobType t) => OnJobButtonPressed?.Invoke(f, v, t);
    }
}
