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
using System.Threading.Tasks;

namespace Harvey.Farm.Fields
{
    public class FieldManager : Singleton<FieldManager>, ISaveSection
    {
        [SerializeField] private int loadPriority = 2;
        public int LoadPriority => loadPriority;

        [SerializeField] Transform fieldParent;

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

        public void Capture(GameSaveData root)
        {
            if (root.Fields == null) root.Fields = new FieldSection();
            root.Fields.Items.Clear();

            foreach (var f in fields)
                root.Fields.Items.Add(FieldMapper.ToSaveData(SerialiseModel(f)));
        }

        public async Task Restore(GameSaveData root)
        {
            if (root.Fields == null) return;

            // Despawn everything
            foreach (var f in new List<FieldController>(fields))
                FieldFactory.Instance.Despawn(f.Model.PrefabGuid, f.gameObject);

            fields.Clear();
            byId.Clear();

            // Respawn
            foreach (var sd in root.Fields.Items)
                await SpawnFromSave(sd);

            Debug.Log($"FieldManager ▸ restored {fields.Count} fields");
        }

        /* ────────────────────────── helpers ────────────────────────── */
        static FieldModel SerialiseModel(FieldController f)
        {
            var builder = f.GetComponent<FieldBuilder>();
            var tiles = builder.Tiles;

            var sb = new System.Text.StringBuilder(tiles.Length);
            foreach (var t in tiles)
                sb.Append(t.IsHarvested ? 'H'
                        : t.IsSeeded ? 'S'
                        : t.IsPlowed ? 'P'
                                        : '.');

            return new FieldModel
            {
                Id = f.GetId(),
                DisplayName = f.Model.DisplayName,
                PrefabGuid = "basic-field",       // TODO remove hard-code
                Position = f.transform.position,
                Width = builder.Grid.Width,
                Height = builder.Grid.Height,
                TileSize = builder.Grid.TileSize,
                CurrentState = (int)f.Current,
                CurrentCropId = f.currentCrop != null ? f.currentCrop.Id : string.Empty,
                TileFlags = sb.ToString()
            };
        }

        static async Task SpawnFromSave(FieldSaveData sd)
        {
            //Spawn new fields
            var model = FieldMapper.FromSaveData(sd);
            var go = await FieldFactory.Instance.SpawnAsync(model.PrefabGuid, null, model.Position);
            if (go == null) return;
            var field = go.GetComponent<FieldController>();

            // Initialize the field with the model
            field.InitFromModel(model);
            Instance.RegisterField(field);
            go.transform.SetParent(Instance.fieldParent, false);
        }
    }
}

