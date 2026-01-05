using System;
using System.Collections;

using Harvey.Farm.Buildings;
using Harvey.Farm.Jobs;
using Harvey.Farm.Movement;
using Harvey.Farm.VehicleScripts;

using UnityEngine;

namespace Harvey.Farm.Jobs.VehicleField
{
    /// <summary>
    /// Step to return the vehicle to its home garage after completing field work.
    /// </summary>
    public class ReturnVehicleStep : IJobStep
    {
        readonly Func<Vehicle> _getVehicle;
        readonly Func<GarageBuilding> _getGarage;
        Coroutine _co;
        bool _finished;

        public ReturnVehicleStep(Func<Vehicle> getVehicle, Func<GarageBuilding> getGarage)
        {
            _getVehicle = getVehicle ?? throw new ArgumentNullException(nameof(getVehicle));
            _getGarage = getGarage ?? throw new ArgumentNullException(nameof(getGarage));
        }

        public bool Tick(float dt)
        {
            Debug.Log($"Ticking ReturnVehicleStep");
            var vehicle = _getVehicle();
            if (vehicle == null) return true; // No vehicle to return

            if (_co == null)
            {
                var mover = vehicle.GetComponent<TractorMover>();
                if (mover == null)
                {
                    Debug.LogWarning($"Vehicle {vehicle.name} is missing TractorMover component");
                    return true; // Skip this step
                }

                Debug.Log($"Returning vehicle {vehicle.name} to garage");
                _co = vehicle.StartCoroutine(ReturnRoutine(mover));
            }

            Debug.Log("Finished? " + _finished);
            return _finished;
        }

        IEnumerator ReturnRoutine(TractorMover mover)
        {
            yield return mover.ReturnToHome();

            var garage = _getGarage();
            garage?.ReturnVehicle(_getVehicle());

            _finished = true;
            _co = null;
        }
    }
}
