using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Harvey.SaveSystem;
using Harvey.Data.Coffee;

/// <summary>
/// Holds every CoffeeCrop in memory and persists them
/// to one JSON file:  Assets/StreamingAssets/coffee_crops.json
/// </summary>
public class CoffeeManager : Singleton<CoffeeManager>, ISaveSection
{
    const string STARTER_FILE = "starter_coffee_crops.json";
    string StarterPath => Path.Combine(Application.streamingAssetsPath, STARTER_FILE);

    readonly List<CoffeeCropData> coffeeCrops = new();

    void Start()
    {
        LoadStarterFile();
        SaveService.Instance.Register(this);
    }

    /* ----------------  public API  -------------- */
    public IReadOnlyList<CoffeeCropData> GetAllCoffeeCrops() => coffeeCrops;

    public CoffeeCropData Create(string name, float growSeconds, float aroma, float acid, float body, string prefabGuid = "basic-coffee")
    {
        var crop = new CoffeeCropData
        {
            Id = Guid.NewGuid().ToString("N"),
            DisplayName = name,
            GrowSeconds = growSeconds,
            Aroma = aroma,
            Acidity = acid,
            Body = body,
            PrefabGuid = prefabGuid
        };

        coffeeCrops.Add(crop);
        return crop;
    }

    public CoffeeCropData GetById(string id) => coffeeCrops.Find(c => c.Id == id);

    /* ---------------- ISaveSection --------------- */

    public string SectionName => "Coffee";

    [Serializable]
    class CoffeeState
    {
        public List<CoffeeCropData> crops;
    }

    public string CaptureJson()
    {
        var state = new CoffeeState
        {
            crops = coffeeCrops
        };
        return JsonUtility.ToJson(state);
    }

    public void RestoreJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return;

        var state = JsonUtility.FromJson<CoffeeState>(json);
        if (state?.crops == null) return;

        coffeeCrops.Clear();
        coffeeCrops.AddRange(state.crops);

        Debug.Log($"CoffeeManager restored {coffeeCrops.Count} crops");
    }

    /* ---------------- internal --------------- */
    void LoadStarterFile()
    {
        if (!File.Exists(StarterPath))
        {
            Debug.LogWarning("Starter crop file missing.");
            return;
        }

        var json = File.ReadAllText(StarterPath);
        var wrap = JsonUtility.FromJson<CoffeeState>(json);
        if (wrap?.crops != null) coffeeCrops.AddRange(wrap.crops);

        Debug.Log($"Loaded {coffeeCrops.Count} starter coffee crops from {STARTER_FILE}");
    }
}
