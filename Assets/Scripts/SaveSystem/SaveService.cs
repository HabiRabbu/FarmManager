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
        const string SAVEFOLDER = "Saves";

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
            var savePath = PathFor(slot);
            Directory.CreateDirectory(Path.GetDirectoryName(savePath));
            File.WriteAllText(savePath, json);
            Debug.Log($"SaveService ▸ wrote {sections.Count} sections to {savePath}");
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

        /* ─────────── save management ─────────── */
        public List<(string name, DateTime timestamp)> GetAllSaves()
        {
            var saves = new List<(string, DateTime)>();
            var dir = Path.Combine(Application.persistentDataPath, SAVEFOLDER);

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var files = Directory.GetFiles(dir, "*" + EXT);

            foreach (var path in files)
            {
                var name = Path.GetFileNameWithoutExtension(path);
                var timestamp = File.GetLastWriteTime(path);
                saves.Add((name, timestamp));
            }

            return saves.OrderByDescending(s => s.Item2).ToList();
        }

        public bool SaveExists(string slot)
        {
            return File.Exists(PathFor(slot));
        }

        public bool RenameSave(string oldName, string newName)
        {
            if (string.IsNullOrEmpty(newName) || oldName == newName)
                return false;

            var oldPath = PathFor(oldName);
            var newPath = PathFor(newName);

            if (!File.Exists(oldPath))
            {
                Debug.LogWarning($"SaveService ▸ Save '{oldName}' does not exist.");
                return false;
            }

            if (File.Exists(newPath))
            {
                Debug.LogWarning($"SaveService ▸ A save named '{newName}' already exists.");
                return false;
            }

            try
            {
                File.Move(oldPath, newPath);
                Debug.Log($"SaveService ▸ Renamed save from '{oldName}' to '{newName}'");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"SaveService ▸ Failed to rename save: {ex.Message}");
                return false;
            }
        }

        public bool DeleteSave(string saveName)
        {
            if (string.IsNullOrEmpty(saveName))
                return false;

            var path = PathFor(saveName);

            if (!File.Exists(path))
            {
                Debug.LogWarning($"SaveService ▸ Save '{saveName}' does not exist.");
                return false;
            }

            try
            {
                File.Delete(path);
                Debug.Log($"SaveService ▸ Deleted save '{saveName}'");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"SaveService ▸ Failed to delete save: {ex.Message}");
                return false;
            }
        }

        /* ─────────── helpers ─────────── */
        public static string PathFor(string slot)
        {
            var dir = Path.Combine(Application.persistentDataPath, SAVEFOLDER);
            return Path.Combine(dir, slot + EXT);
        }

    }
}
