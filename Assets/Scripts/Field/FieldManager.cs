using System.Collections.Generic;
using UnityEngine;
using Harvey.Farm.Utilities;
using Harvey.Farm.Events;
using Harvey.SaveSystem;
using System;
using Harvey.Data.Fields;
using System.Text;
using Harvey.Farm.Factory;
using Harvey.Data.Coffee;

namespace Harvey.Farm.Fields
{
    public class FieldManager : Singleton<FieldManager>, ISaveSection
    {
        [Header("Field Settings")]
        [SerializeField] public float tileSize;

        private FieldController current = null;
        readonly List<FieldController> fields = new();

        readonly Dictionary<string, FieldController> byId = new();

        void Start()
        {
            SaveService.Instance.Register(this);
        }

        public FieldController GetById(string id) =>
                (id != null && byId.TryGetValue(id, out var f)) ? f : null;

        public void RegisterField(FieldController f)
        {
            if (!fields.Contains(f)) fields.Add(f);
            if (!string.IsNullOrEmpty(f.GetId())) byId[f.GetId()] = f;
        }
        public void UnregisterField(FieldController f)
        {
            fields.Remove(f);
            if (!string.IsNullOrEmpty(f.GetId())) byId.Remove(f.GetId());
        }

        public FieldController GetFieldAtPoint(Vector3 worldPos)
        {
            FieldController foundField = fields.Find(f => f.GetComponent<FieldBuilder>().ContainsPoint(worldPos));
            current = foundField ?? null;
            //TODO: Some highlighting one day?  current.ShowOutline(true);
            return current;
        }

        /* ---------------- ISaveSection --------------- */

        public string SectionName => "Fields";

        [Serializable]
        class FieldState
        {
            public List<FieldSaveData> savableFields;
        }

        public string CaptureJson()
        {
            List<FieldSaveData> list = new();

            foreach (var f in fields)
                list.Add(SerialiseField(f));

            return JsonUtility.ToJson(new FieldState
            {
                savableFields = list
            });
        }

        public void RestoreJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return;

            var state = JsonUtility.FromJson<FieldState>(json);
            if (state?.savableFields == null) return;

            foreach (var f in new List<FieldController>(fields))
                FieldFactory.Instance.Despawn("basic-field", f.gameObject); //TODO: Don't hardcode this
            fields.Clear();
            byId.Clear();

            foreach (var dto in state.savableFields)
                DeserialiseField(dto);

            Debug.Log($"FieldManager - rebuilt {state.savableFields.Count} fields from save");
        }

        /* ────────────────────────── helpers ────────────────────────── */
        static FieldSaveData SerialiseField(FieldController f)
        {
            var builder = f.GetComponent<FieldBuilder>();
            var grid = builder.Grid;
            var tiles = builder.Tiles;

            var flags = new StringBuilder(tiles.Length);
            foreach (var t in tiles)
                flags.Append(t.IsHarvested ? 'H'
                           : t.IsSeeded ? 'S'
                           : t.IsPlowed ? 'P'
                           : '.');

            return new FieldSaveData
            {
                Id = f.GetId(),
                Width = grid.Width,
                Height = grid.Height,
                TileSize = grid.TileSize,
                PrefabGuid = "basic-field", //TODO: f.PrefabGuid or something idk - Just dont hardcode
                TilePrefabGuid = "basic-field-tile",
                Position = f.transform.position,

                CurrentState = (int)f.Current,
                CurrentCropId = f.currentCrop?.Id,
                TileFlags = flags.ToString()
            };
        }

        //TODO: Spawning too many fields, breaking everything. Fields aren't being respawned properly.
        static void DeserialiseField(FieldSaveData d)
        {
            /* A. get or create the field ------------------------------------------------ */
            var field = Instance.GetById(d.Id);
            if (field == null)
            {
                var go = FieldFactory.Instance.Spawn(d, null, d.Position);
                field = go.GetComponent<FieldController>();
                field.SetId(d.Id);
                Instance.RegisterField(field);
            }
            else field.transform.position = d.Position;

            /* B. regenerate tiles ------------------------------------------------------- */
            var builder = field.GetComponent<FieldBuilder>();
            builder.BuildFromData(d);                    // Tiles[] is freshly rebuilt
            field.runtime.Initialize(builder.Tiles);

            /* C. restore high-level state ---------------------------------------------- */
            field.currentState = (FieldController.State)d.CurrentState;
            field.currentCrop = string.IsNullOrEmpty(d.CurrentCropId)
                                 ? null
                                 : CoffeeManager.Instance.GetById(d.CurrentCropId);

            /* D. replay per-tile flags -------------------------------------------------- */
            ApplyTileFlags(builder.Tiles, d.TileFlags, field.currentCrop);
        }

        /* ───────────────────────── helpers ───────────────────────── */
        static void ApplyTileFlags(FieldTile[] tiles,
                                   string flags,
                                   CoffeeCropData crop)
        {
            int count = Mathf.Min(tiles.Length, flags.Length);

            for (int i = 0; i < count; i++)
            {
                var t = tiles[i];
                t.ResetTile();

                switch (flags[i])
                {
                    case 'P':
                        t.Plow();
                        break;

                    case 'S':
                        t.Plow();
                        t.Seed(crop);
                        break;

                    case 'H':
                        t.Plow();
                        t.Seed(crop);
                        t.Harvest();
                        break;
                }
            }
        }



    }
}

