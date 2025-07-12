using System;
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
            var wrapper = new Wrapper();
            foreach (var s in sections)
                wrapper.Sections.Add(new Wrapper.Section
                {
                    key = s.SectionName,
                    json = s.CaptureJson()
                });

            File.WriteAllText(PathFor(slot),
                              JsonUtility.ToJson(wrapper, true));
        }

        public void LoadGame(string slot = "autosave")
        {
            string path = PathFor(slot);
            if (!File.Exists(path))
            {
                Debug.LogWarning($"Save file not found: {path}");
                return;
            }

            var wrapper = JsonUtility.FromJson<Wrapper>(File.ReadAllText(path));

            foreach (var s in sections)
            {
                if (wrapper.TryGet(s.SectionName, out string json))
                    s.RestoreJson(json);
            }
        }

        /* ------------- helpers --------------- */
        string PathFor(string slot) =>
            Path.Combine(Application.persistentDataPath, slot + EXT);

        [Serializable]
        class Wrapper
        {
            public List<Section> Sections = new();

            public bool TryGet(string key, out string json)
            {
                var sec = Sections.Find(s => s.key == key);
                json = sec?.json;
                return sec != null;
            }

            [Serializable]
            public class Section
            {
                public string key;
                public string json;
            }
        }
    }
}
