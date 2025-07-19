using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Harvey.Farm.Utilities;
using System.Linq;
using Harvey.Farm.Events;

namespace Harvey.Farm.Utilities
{
    /// <summary>
    /// Preloads Addressable prefabs automatically by scanning all available addresses.
    /// Attach this component to GameManager or any GameObject.
    /// </summary>
    public class PrefabPreloader : MonoBehaviour
    {
        [Header("Preload Settings")]
        [SerializeField] private bool preloadOnStart = true;
        [SerializeField] private bool showLoadingProgress = true;
        [SerializeField] private bool preloadOnlyGameObjects = true;

        [Header("Optional Filters")]
        [SerializeField] private string[] includeLabels = { "Buildings", "Vehicles", "Workers", "Implements", "Fields", "Crops" };
        [SerializeField] private string[] excludeLabels = { "Audio", "Textures", "Materials" };
        [SerializeField] private bool useFilters = false;

        [Header("Debug")]
        [SerializeField] private bool logFoundAssets = false;

        // State
        public bool IsPreloadComplete { get; private set; } = false;
        public int TotalAssetsFound { get; private set; } = 0;
        public int AssetsPreloaded { get; private set; } = 0;
        public List<string> PreloadedAssets { get; private set; } = new List<string>();

        async void Start()
        {
            if (preloadOnStart)
            {
                await PreloadAllAddressableAssets();
            }
        }

        public async Task<bool> PreloadAllAddressableAssets()
        {
            Debug.Log("🔍 PrefabPreloader: Scanning for Addressable assets...");

            try
            {
                // Get all addressable asset locations
                var locations = await GetAllAddressableLocations();

                if (locations.Count == 0)
                {
                    Debug.LogWarning("⚠️ PrefabPreloader: No addressable assets found!");
                    return false;
                }

                TotalAssetsFound = locations.Count;
                Debug.Log($"📦 PrefabPreloader: Found {TotalAssetsFound} addressable assets");

                // Preload each asset
                var pool = AddressablePoolManager.Instance;
                AssetsPreloaded = 0;
                PreloadedAssets.Clear();

                foreach (var location in locations)
                {
                    await PreloadSingleAsset(pool, location.PrimaryKey);
                }

                IsPreloadComplete = true;
                GameEvents.PreloadComplete();

                Debug.Log($"✅ PrefabPreloader: Preloading complete! Loaded {AssetsPreloaded}/{TotalAssetsFound} assets");
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ PrefabPreloader: Failed to preload assets: {e.Message}");
                return false;
            }
        }

        private async Task<List<UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation>> GetAllAddressableLocations()
        {
            var allLocations = new List<UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation>();

            if (useFilters && includeLabels.Length > 0)
            {
                // Load by labels if filters are enabled
                foreach (var label in includeLabels)
                {
                    try
                    {
                        var handle = Addressables.LoadResourceLocationsAsync(label);
                        var locations = await handle.Task;

                        foreach (var location in locations)
                        {
                            if (ShouldIncludeAsset(location))
                            {
                                allLocations.Add(location);
                            }
                        }

                        Addressables.Release(handle);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogWarning($"⚠️ PrefabPreloader: Failed to load locations for label '{label}': {e.Message}");
                    }
                }
            }
            else
            {
                // Load all addressable locations
                try
                {
                    var handle = Addressables.LoadResourceLocationsAsync("default");
                    var locations = await handle.Task;

                    foreach (var location in locations)
                    {
                        if (ShouldIncludeAsset(location))
                        {
                            allLocations.Add(location);
                        }
                    }

                    Addressables.Release(handle);
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"⚠️ PrefabPreloader: Failed to load default locations: {e.Message}");

                    // Fallback: try loading without labels
                    var handle = Addressables.LoadResourceLocationsAsync(typeof(GameObject));
                    var locations = await handle.Task;

                    foreach (var location in locations)
                    {
                        if (ShouldIncludeAsset(location))
                        {
                            allLocations.Add(location);
                        }
                    }

                    Addressables.Release(handle);
                }
            }

            return allLocations;
        }

        private bool ShouldIncludeAsset(UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation location)
        {
            // Only include GameObjects if specified
            if (preloadOnlyGameObjects && location.ResourceType != typeof(GameObject))
            {
                return false;
            }

            // Check exclude labels
            if (useFilters && excludeLabels.Length > 0)
            {
                foreach (var excludeLabel in excludeLabels)
                {
                    if (location.PrimaryKey.Contains(excludeLabel))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private async Task PreloadSingleAsset(AddressablePoolManager pool, string guid)
        {
            try
            {
                await pool.PreloadAsync(guid);
                AssetsPreloaded++;
                PreloadedAssets.Add(guid);

                if (showLoadingProgress)
                {
                    Debug.Log($"📦 Preloaded: {guid} ({AssetsPreloaded}/{TotalAssetsFound})");
                }

                // Update progress
                float progress = TotalAssetsFound > 0 ? (float)AssetsPreloaded / TotalAssetsFound : 0f;

                GameEvents.PreloadProgress(progress);
                GameEvents.AssetPreloaded(guid);

                if (logFoundAssets)
                {
                    Debug.Log($"  ✅ Found asset: {guid}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"⚠️ Failed to preload '{guid}': {e.Message}");
            }
        }



        /* ---------------------- Debugging and Manual Controls --------------------- */

        /// <summary>
        /// Manual trigger for preloading.
        /// </summary>
        [ContextMenu("Preload All Assets")]
        public async void ManualPreload()
        {
            await PreloadAllAddressableAssets();
        }

        /// <summary>
        /// Debug method to show all found assets.
        /// </summary>
        [ContextMenu("Debug: List All Found Assets")]
        public async void DebugListAssets()
        {
            var locations = await GetAllAddressableLocations();
            Debug.Log($"🔍 Found {locations.Count} addressable assets:");

            foreach (var location in locations)
            {
                Debug.Log($"  📦 {location.PrimaryKey} ({location.ResourceType.Name})");
            }
        }

        /// <summary>
        /// Check which assets are currently cached.
        /// </summary>
        [ContextMenu("Debug: Check Cached Assets")]
        public void DebugCachedAssets()
        {
            var pool = AddressablePoolManager.Instance;
            Debug.Log("🔍 Checking cached assets:");

            foreach (var guid in PreloadedAssets)
            {
                bool cached = pool.GetCachedPrefab(guid) != null;
                string status = cached ? "✅ Cached" : "❌ Not cached";
                Debug.Log($"  {guid}: {status}");
            }
        }
    }
}