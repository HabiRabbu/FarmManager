using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Harvey.SaveSystem
{
    // Should save to ...\AppData\LocalLow\DefaultCompany\FarmManager
    public class SaveService : Singleton<SaveService>
    {
        const string EXT = ".json";
        readonly List<ISaveSection> sections = new();
        GameSaveData cache = new();

        /* ─────────── registration ─────────── */
        public void Register(ISaveSection section)
        {
            if (!sections.Contains(section))
                sections.Add(section);
        }

        /* ─────────────  API  ────────────── */
        public void SaveGame(string slot = "autosave")
        {
            foreach (var s in sections)
                s.Capture(cache);

            var json = JsonUtility.ToJson(cache, true);
            File.WriteAllText(PathFor(slot), json);
            Debug.Log($"SaveService ▸ wrote {sections.Count} sections to {PathFor(slot)}");
        }

        public async Task LoadGame(string slot = "autosave")
        {
            var path = PathFor(slot);
            if (!File.Exists(path))
            {
                Debug.LogWarning($"SaveService ▸ no save found at {path}");
                return;
            }

            cache = JsonUtility.FromJson<GameSaveData>(File.ReadAllText(path)) ?? new GameSaveData();

            var sortedSections = sections.OrderBy(s => s.LoadPriority).ToList();

            foreach (var s in sortedSections)
                await s.Restore(cache);

            Debug.Log($"SaveService ▸ loaded {sections.Count} sections from {path}");
        }

        /* ─────────── helpers ─────────── */
        string PathFor(string slot) =>
            Path.Combine(Application.persistentDataPath, slot + EXT);
    }
}
