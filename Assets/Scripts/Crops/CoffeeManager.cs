using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Harvey.SaveSystem;
using Harvey.Data.Coffee;
using System.Threading.Tasks;

/// <summary>
/// Holds every CoffeeCrop in memory and persists them
/// to one JSON file:  Assets/StreamingAssets/coffee_crops.json
/// </summary>
public class CoffeeManager : Singleton<CoffeeManager>, ISaveSection
{
    [SerializeField] public int LoadPriority { get; } = 1;

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

    public void Capture(GameSaveData root)
    {
        if (root.Coffee == null) root.Coffee = new CoffeeSection();
        root.Coffee.Crops.Clear();
        root.Coffee.Crops.AddRange(coffeeCrops);
    }

    public Task Restore(GameSaveData root)
    {
        if (root.Coffee == null) return Task.FromException(new InvalidOperationException("Coffee section missing in save data"));

        coffeeCrops.Clear();
        coffeeCrops.AddRange(root.Coffee.Crops);

        Debug.Log($"CoffeeManager ▸ restored {coffeeCrops.Count} crops");
        return Task.CompletedTask;
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
        var wrap = JsonUtility.FromJson<CoffeeSection>(json);
        if (wrap?.Crops != null) coffeeCrops.AddRange(wrap.Crops);

        Debug.Log($"Loaded {coffeeCrops.Count} starter coffee crops from {STARTER_FILE}");
    }
}
