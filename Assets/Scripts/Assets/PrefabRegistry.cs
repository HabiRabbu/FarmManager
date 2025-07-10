using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Roast/Prefab Registry")]
public class PrefabRegistry : ScriptableObject
{
    [System.Serializable]
    public struct Entry
    {
        public string guid;
        public GameObject prefab;
    }

    [SerializeField] List<Entry> entries = new();

    readonly Dictionary<string, GameObject> map = new();

    void OnEnable()
    {
        map.Clear();
        foreach (var e in entries)
            if (e.prefab) map[e.guid] = e.prefab;
    }

    public GameObject Get(string guid) =>
        map.TryGetValue(guid, out var pf) ? pf : null;
}
