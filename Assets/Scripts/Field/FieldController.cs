using System.Collections;
using Harvey.Farm.Crops;
using Harvey.Farm.Events;
using UnityEngine;

namespace Harvey.Farm.Fields
{
    public class FieldController : MonoBehaviour
    {
        public FieldDefinition definition;
        public CropDefinition currentCrop;
        public State currentState = State.Idle;

        FieldBuilder builder;
        public FieldRuntimeState runtime;

        // Property Getters
        public float TilesCompletedFraction => runtime.Completion;
        public bool Is(State s) => currentState == s;
        public State Current => currentState;
        public FieldDefinition Definition => definition;


        void Awake()
        {
            builder = GetComponent<FieldBuilder>();
            runtime = GetComponent<FieldRuntimeState>();
        }

        void Start()
        {
            builder.Build();
            runtime.Initialize(builder.Tiles);
            gameObject.name = $"Field - {definition.fieldName}";
            FieldManager.Instance.RegisterField(this);
        }

        void OnEnable()
        {
            GameEvents.OnTilePlowed += HandleTilePlowed;
            GameEvents.OnTileSeeded += HandleTileSeeded;
            GameEvents.OnTileHarvested += HandleTileHarvested;
        }

        void OnDisable()
        {
            GameEvents.OnTilePlowed -= HandleTilePlowed;
            GameEvents.OnTileSeeded -= HandleTileSeeded;
            GameEvents.OnTileHarvested -= HandleTileHarvested;
        }

        void HandleTilePlowed(FieldTile tile)
        {
            if (tile.transform.parent != transform) return;
            HandleTileCompleted();
        }

        void HandleTileSeeded(FieldTile tile, CropDefinition crop)
        {
            if (tile.transform.parent != transform) return;
            HandleTileCompleted();
        }

        void HandleTileHarvested(FieldTile tile)
        {
            if (tile.transform.parent != transform) return;
            HandleTileCompleted();
        }

        public bool Needs(JobType job) => job switch
        {
            JobType.Plow => currentState is State.Idle or State.Harvested,
            JobType.Seed => currentState == State.Plowed,
            JobType.Harvest => currentState == State.ReadyToHarvest,
            _ => false
        };

        public void BeginJob(JobType job, CropDefinition crop = null)
        {
            currentState = job switch
            {
                JobType.Plow => State.Plowing,
                JobType.Seed => State.Seeding,
                JobType.Harvest => State.Harvesting,
                _ => currentState
            };

            if (job == JobType.Seed) currentCrop = crop;
            runtime.ResetProgress();
        }

        public void HandleTileCompleted()
        {
            if (!runtime.Advance()) return;

            switch (currentState)
            {
                case State.Plowing:
                    currentState = State.Plowed;
                    GameEvents.FieldCompleted(this);
                    break;
                case State.Seeding:
                    currentState = State.Seeded;
                    GameEvents.FieldCompleted(this);
                    StartCoroutine(GrowRoutine());
                    break;
                case State.Harvesting:
                    currentState = State.Harvested;
                    GameEvents.FieldHarvested(this);
                    break;
            }
        }

        public FieldTile[] GetSerpentineTiles()
        {
            var tiles = builder.Tiles;
            if (definition.width >= definition.height)
                return builder.Grid.BuildSerpentineRows(tiles);
            else
                return builder.Grid.BuildSerpentineColumns(tiles);
        }

        IEnumerator GrowRoutine()
        {
            currentState = State.Growing;

            float step = currentCrop.growSeconds / 2f;
            yield return new WaitForSeconds(step);
            foreach (var t in builder.Tiles) t.SetStage(1);
            yield return new WaitForSeconds(step);
            foreach (var t in builder.Tiles) t.SetStage(2);

            currentState = State.ReadyToHarvest;
            GameEvents.FieldGrown(this);
        }

        public enum State { Idle, Plowing, Plowed, Seeding, Seeded, Growing, ReadyToHarvest, Harvesting, Harvested }
    }
}