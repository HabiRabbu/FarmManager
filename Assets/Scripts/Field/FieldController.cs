using System;
using System.Collections;
using System.Threading.Tasks;

using Harvey.Data.Coffee;
using Harvey.Farm.Events;
using Harvey.Farm.Jobs;

using UnityEngine;

namespace Harvey.Farm.Fields
{
    public class FieldController : MonoBehaviour
    {
        public FieldDefinition definition;
        public FieldModel Model { get; private set; }

        public CoffeeCropData currentCrop;
        public State currentState = State.Idle;

        FieldBuilder builder;
        public FieldRuntimeState runtime;

        // Property Getters
        public float TilesCompletedFraction => runtime.Completion;
        public bool Is(State s) => currentState == s;
        public State Current => currentState;
        public string DisplayName => Model?.DisplayName ?? definition?.FieldName ?? "Unknown Field";

        // GuidBehaviour
        GuidBehaviour guidBehaviour;
        public string GetId() => guidBehaviour.GetId();
        public void SetId(string newId) => guidBehaviour.SetId(newId);

        void Awake()
        {
            builder = GetComponent<FieldBuilder>();
            runtime = GetComponent<FieldRuntimeState>();
            guidBehaviour = GetComponent<GuidBehaviour>();
        }

        async void Start()
        {
            if (Model == null && definition != null)
            {
                Model = FieldMapper.FromDefinition(definition, guidBehaviour.GetId(), transform.position);
                FieldManager.Instance.RegisterField(this);

                await BuildField();
                gameObject.name = $"Field - {definition.FieldName}";
            }
        }

        private async Task BuildField()
        {
            await builder.BuildAsync(); // Make this async
            runtime.Initialize(builder.Tiles);
            Debug.Log($"✅ FieldController: Field built and runtime initialized with {builder.Tiles?.Length ?? 0} tiles");
        }

        public async void InitFromModel(FieldModel model)
        {
            if (model == null) return;

            Model = model;
            guidBehaviour.SetId(model.Id);
            FieldManager.Instance.RegisterField(this);

            // Wait for build to complete before initializing runtime
            await builder.BuildFromModelAsync(model);
            runtime.Initialize(builder.Tiles);
            FromModelParser(model);
            gameObject.name = $"Field - {Model.DisplayName}";
        }

        void FromModelParser(FieldModel model)
        {
            //State
            currentState = (State)Model.CurrentState;

            //Current Crop
            currentCrop = CoffeeManager.Instance.GetById(Model.CurrentCropId);

            //Completion Amount
            foreach (var c in Model.TileFlags)
            {
                if (c != '.')
                    runtime.Advance();
            }

            //Tile Flags
            var tiles = builder.Tiles;
            var tileFlags = model.TileFlags;
            int count = Mathf.Min(tiles.Length, tileFlags.Length);

            for (int i = 0; i < count; i++)
            {
                var t = tiles[i];
                t.ResetTile();

                switch (tileFlags[i])
                {
                    case 'P':
                        t.Plow();
                        break;

                    case 'S':
                        t.Plow();
                        t.Seed(currentCrop);
                        break;

                    case 'H':
                        t.Plow();
                        t.Seed(currentCrop);
                        t.Harvest();
                        break;
                }
            }
            // Start GrowRoutine if Seeded
            if (currentState == State.Seeded || currentState == State.Growing)
            {
                StartCoroutine(GrowRoutine());
            }


            //Anything else...?
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
        void OnDestroy()
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

        void HandleTileSeeded(FieldTile tile, CoffeeCropData crop)
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
            JobType.Plow => currentState is State.Idle or State.Harvested or State.Plowing,
            JobType.Seed => currentState is State.Plowed or State.Seeding,
            JobType.Harvest => currentState is State.ReadyToHarvest,
            _ => false
        };

        public JobType? GetNeededJobType()
        {
            return currentState switch
            {
                State.Idle or State.Harvested or State.Plowing => JobType.Plow,
                State.Plowed or State.Seeding => JobType.Seed,
                State.ReadyToHarvest => JobType.Harvest,
                _ => null
            };
        }

        public void BeginJob(JobType job, CoffeeCropData crop = null)
        {
            var newState = job switch
            {
                JobType.Plow => State.Plowing,
                JobType.Seed => State.Seeding,
                JobType.Harvest => State.Harvesting,
                _ => currentState
            };

            // Only reset progress if we're transitioning to a new job type
            bool shouldResetProgress = currentState != newState;

            currentState = newState;
            if (job == JobType.Seed) currentCrop = crop;

            if (shouldResetProgress)
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
            if (Model.Width >= Model.Height)
                return builder.Grid.BuildSerpentineRows(tiles);
            else
                return builder.Grid.BuildSerpentineColumns(tiles);
        }

        public FieldTile GetNearestAvailableTile(JobType type, Vector3 fromPos)
        {
            float best = float.MaxValue;
            FieldTile bestTile = null;

            foreach (var t in builder.Tiles)
            {
                if (t.IsReserved) continue;
                if (type == JobType.Plow && t.IsPlowed) continue;
                if (type == JobType.Seed && t.IsSeeded) continue;
                if (type == JobType.Harvest && t.IsHarvested) continue;

                float d = Vector3.SqrMagnitude(t.WorldPosition - fromPos);
                if (d < best)
                {
                    best = d;
                    bestTile = t;
                }
            }

            if (bestTile != null && bestTile.TryReserve())
                return bestTile;

            return null;
        }

        IEnumerator GrowRoutine()
        {
            currentState = State.Growing;

            float step = currentCrop.GrowSeconds / 2f;
            yield return new WaitForSeconds(step);
            foreach (var t in builder.Tiles) t.SetStage(1);
            yield return new WaitForSeconds(step);
            foreach (var t in builder.Tiles) t.SetStage(2);

            currentState = State.ReadyToHarvest;
            GameEvents.FieldGrown(this);
        }

        // ---------- Radiaul Menu Support ----------
        public void RadialOpenFieldInfo() => GameEvents.RadialFieldInfoOpened(this);
        public void RadialOpenFieldTractor() => GameEvents.RadialFieldTractorOpened(this);
        public void RadialOpenFieldWorkers() => GameEvents.RadialFieldWorkersOpened(this);
        // -------------------------------------------

        /// <summary>
        /// Gets the current job type being performed on this field based on its state.
        /// Returns null if no active job is in progress.
        /// </summary>
        public JobType? GetCurrentJobType()
        {
            return currentState switch
            {
                State.Plowing => JobType.Plow,
                State.Seeding => JobType.Seed,
                State.Harvesting => JobType.Harvest,
                _ => null
            };
        }

        public enum State { Idle, Plowing, Plowed, Seeding, Seeded, Growing, ReadyToHarvest, Harvesting, Harvested }
    }
}