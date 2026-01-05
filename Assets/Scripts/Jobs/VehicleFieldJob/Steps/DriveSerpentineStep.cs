using System;
using System.Collections.Generic;

using Harvey.Data.Coffee;
using Harvey.Farm.Fields;
using Harvey.Farm.Jobs;
using Harvey.Farm.Movement;
using Harvey.Farm.VehicleScripts;

using UnityEngine;

namespace Harvey.Farm.Jobs.VehicleField
{
    /// <summary>
    /// Step that drives a vehicle in a serpentine pattern across field tiles while performing actions.
    /// </summary>
    public class DriveSerpentineStep : IJobStep
    {
        readonly FieldController _field;
        readonly JobType _jobType;
        readonly Func<Vehicle> _getVehicle;
        readonly CoffeeCropData _crop;

        readonly Action<int> _saveTileIndex;   // writes progress back
        readonly int _startTileIndex;  // resume point

        TractorMover _mover;
        Vector3[] _waypoints;
        int _tileIndex;
        bool _isInitialised;

        public DriveSerpentineStep(FieldController field,
                                   JobType jobType,
                                   CoffeeCropData crop,
                                   Func<Vehicle> getVehicle,
                                   int resumeTile,
                                   Action<int> saveIndex)
        {
            _field = field ?? throw new ArgumentNullException(nameof(field));
            _jobType = jobType;
            _getVehicle = getVehicle ?? throw new ArgumentNullException(nameof(getVehicle));
            _startTileIndex = resumeTile;
            _crop = crop;
            _saveTileIndex = saveIndex ?? throw new ArgumentNullException(nameof(saveIndex));
        }

        public bool Tick(float dt)
        {
            if (!_isInitialised)
            {
                if (!InitialiseMovement()) return false;
                _isInitialised = true;
            }

            return _tileIndex >= _waypoints.Length;
        }

        bool InitialiseMovement()
        {
            var vehicle = _getVehicle() as Tractor;
            if (vehicle == null) return false;

            _mover = vehicle.GetComponent<TractorMover>();
            if (_mover == null)
            {
                Debug.LogError($"Vehicle {vehicle.name} is missing TractorMover component");
                return false;
            }

            var serpentineTiles = _field.GetSerpentineTiles();
            var waypointsList = new List<Vector3>(serpentineTiles.Length);
            for (int i = 0; i < serpentineTiles.Length; ++i)
                waypointsList.Add(serpentineTiles[i].WorldPosition);

            _waypoints = waypointsList.ToArray(); // Keep array for length check

            System.Action<int> perTileAction = _jobType switch
            {
                JobType.Plow => i =>
                {
                    var tile = serpentineTiles[i];
                    if (!tile.IsPlowed) tile.Plow();
                }
                ,
                JobType.Seed => i =>
                {
                    var tile = serpentineTiles[i];
                    if (!tile.IsSeeded) tile.Seed(_crop);
                }
                ,
                JobType.Harvest => i =>
                {
                    var tile = serpentineTiles[i];
                    if (!tile.IsHarvested) tile.Harvest();
                }
                ,
                _ => null
            };

            _mover.StartCoroutine(_mover.MoveAlong(waypointsList, i =>
            {
                perTileAction?.Invoke(i);
                _tileIndex = i + 1;
                _saveTileIndex(_tileIndex);
            }, _startTileIndex));

            return true;
        }
    }
}
