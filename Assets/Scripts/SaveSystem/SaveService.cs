using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Harvey.SaveSystem
{
    // Should save to C:\Users\harve_f1jqi6p\AppData\LocalLow\DefaultCompany\FarmManager
    // or in the Editor: Assets/StreamingAssets/autosave.json
    public class SaveService : Singleton<SaveService>
    {
        const string EXT = ".json";
        readonly List<ISaveSection> sections = new();

        public void Register(ISaveSection s) => sections.Add(s);

        /* ------------- public API ------------- */
        public void SaveGame(string slot = "autosave")
        {
            var dict = new Dictionary<string, object>();
            foreach (var s in sections)
                dict[s.SectionName] = s.CaptureState();

            var json = JsonUtility.ToJson(new Wrapper(dict), true);
            File.WriteAllText(PathFor(slot), json);
            Debug.Log($"Saved {sections.Count} sections → {PathFor(slot)}");
        }

        public void LoadGame(string slot = "autosave")
        {
            string path = PathFor(slot);
            if (!File.Exists(path)) { Debug.LogWarning("Save not found"); return; }

            var json = File.ReadAllText(path);
            var wrapper = JsonUtility.FromJson<Wrapper>(json);

            foreach (var s in sections)
                if (wrapper.TryGet(s.SectionName, out var data))
                    s.RestoreState(data);
        }

        /* ------------- helpers --------------- */
        string PathFor(string slot) =>
            Path.Combine(Application.persistentDataPath, slot + EXT);

        [System.Serializable]
        class Wrapper
        {
            public List<Section> Sections = new();

            public Wrapper() { }
            public Wrapper(Dictionary<string, object> dict)
            {
                foreach (var kv in dict)
                    Sections.Add(new Section { key = kv.Key, json = JsonUtility.ToJson(kv.Value) });
            }

            public bool TryGet(string key, out object obj)
            {
                var sec = Sections.Find(s => s.key == key);
                if (sec != null)
                {
                    obj = JsonUtility.FromJson(sec.json, typeof(object));
                    return true;
                }
                obj = null; return false;
            }

            [System.Serializable]
            public class Section
            {
                public string key;
                public string json;
            }
        }
    }
}
