using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableManager
{
    private Dictionary<int, string> levelKeyMap = new Dictionary<int, string>();
    private Dictionary<int, string> levelToPackMap = new Dictionary<int, string>();
    private Dictionary<string, AsyncOperationHandle> loadedPackHandles = new Dictionary<string, AsyncOperationHandle>();
    private Dictionary<string, AsyncOperationHandle<LevelData>> loadedLevelHandles = new Dictionary<string, AsyncOperationHandle<LevelData>>();
    private int totalLevels = 10;
    private AsyncOperationHandle<IList<UnityEngine.Object>> menuUIHandle;
    private AsyncOperationHandle<IList<UnityEngine.Object>> mapAssetsHandle;
    private bool isMenuUILoaded = false;
    private bool isMapAssetsLoaded = false;
    
    public void Init()
    {
        for (int i = 1; i <= totalLevels; i++)
        {
            levelKeyMap[i] = $"Level{i}";
            levelToPackMap[i] = i <= 5 ? "LevelPack1" : "LevelPack2";
        }
    }

    public void LoadLevelAsync(int levelNumber, System.Action<LevelData> onComplete, System.Action<string> onError = null)
    {
        if (!levelKeyMap.ContainsKey(levelNumber))
        {
            onError?.Invoke($"Level {levelNumber} not found in configuration");
            return;
        }

        string levelKey = levelKeyMap[levelNumber];
        string packKey = levelToPackMap[levelNumber];

        if (!loadedPackHandles.ContainsKey(packKey))
        {
            var packHandle = Addressables.LoadAssetsAsync<UnityEngine.Object>(packKey, null);
            loadedPackHandles[packKey] = packHandle;

            packHandle.Completed += handle => {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log($"Pack {packKey} loaded successfully with {handle.Result.Count} assets");
                    LoadLevelData(levelKey, onComplete, onError);
                }
                else
                {
                    onError?.Invoke($"Failed to load pack {packKey}: {handle.OperationException?.Message}");
                }
            };
        }
        else
        {
            LoadLevelData(levelKey, onComplete, onError);
        }
    }    private void LoadLevelData(string levelKey, System.Action<LevelData> onComplete, System.Action<string> onError)
    {
        var levelHandle = Addressables.LoadAssetAsync<LevelData>(levelKey);
        loadedLevelHandles[levelKey] = levelHandle;

        levelHandle.Completed += handle => {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                onComplete?.Invoke(handle.Result);
            }
            else
            {
                onError?.Invoke($"Failed to load level {levelKey}: {handle.OperationException?.Message}");
            }
        };
    }    public void UnloadPackage(string packKey)
    {
        Debug.Log($"Starting to unload package: {packKey}");
        
        var levelsToUnload = new List<string>();
        foreach (var kvp in levelKeyMap)
        {
            if (levelToPackMap[kvp.Key] == packKey)
            {
                string levelKey = kvp.Value;
                if (loadedLevelHandles.ContainsKey(levelKey))
                {
                    Addressables.Release(loadedLevelHandles[levelKey]);
                    levelsToUnload.Add(levelKey);
                    Debug.Log($"Released level handle: {levelKey}");
                }
            }
        }
        
        foreach (var levelKey in levelsToUnload)
        {
            loadedLevelHandles.Remove(levelKey);
        }
        
        if (loadedPackHandles.ContainsKey(packKey))
        {
            Addressables.Release(loadedPackHandles[packKey]);
            loadedPackHandles.Remove(packKey);
            Debug.Log($"Released pack handle: {packKey}");
        }
     
        
        Debug.Log($"Package {packKey} completely unloaded");
    }    public void UnloadAllPackages()
    {
        foreach (var kvp in loadedLevelHandles)
        {
            Addressables.Release(kvp.Value);
        }
        loadedLevelHandles.Clear();
        
        foreach (var kvp in loadedPackHandles)
        {
            Addressables.Release(kvp.Value);
        }
        loadedPackHandles.Clear();
        
        UnloadMenuUI();
        UnloadMapAssets();

        
        Debug.Log("All packages unloaded");
    }

    public bool IsPackLoaded(string packKey)
    {
        return loadedPackHandles.ContainsKey(packKey);
    }

    public void Dispose()
    {
        UnloadAllPackages();
    }

    public void LoadMenuUIAsync(System.Action onComplete = null, System.Action<string> onError = null)
    {
        if (isMenuUILoaded)
        {
            onComplete?.Invoke();
            return;
        }

        Debug.Log("Loading MenuUI assets...");
        menuUIHandle = Addressables.LoadAssetsAsync<UnityEngine.Object>("MenuUI", null);
        
        menuUIHandle.Completed += handle => {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                isMenuUILoaded = true;
                Debug.Log($"MenuUI loaded successfully with {handle.Result.Count} assets");
                onComplete?.Invoke();
            }
            else
            {
                Debug.LogError($"Failed to load MenuUI: {handle.OperationException?.Message}");
                onError?.Invoke($"Failed to load MenuUI: {handle.OperationException?.Message}");
            }
        };
    }
    
    public void UnloadMenuUI()
    {
        if (isMenuUILoaded && menuUIHandle.IsValid())
        {
            Addressables.Release(menuUIHandle);
            isMenuUILoaded = false;
            Debug.Log("MenuUI unloaded");
        }
    }
    
    public void LoadMapAssetsAsync(System.Action onComplete = null, System.Action<string> onError = null)
    {
        if (isMapAssetsLoaded)
        {
            onComplete?.Invoke();
            return;
        }

        Debug.Log("Loading MapAssets...");
        mapAssetsHandle = Addressables.LoadAssetsAsync<UnityEngine.Object>("MapAssets", null);
        
        mapAssetsHandle.Completed += handle => {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                isMapAssetsLoaded = true;
                Debug.Log($"MapAssets loaded successfully with {handle.Result.Count} assets");
                onComplete?.Invoke();
            }
            else
            {
                Debug.LogError($"Failed to load MapAssets: {handle.OperationException?.Message}");
                onError?.Invoke($"Failed to load MapAssets: {handle.OperationException?.Message}");
            }
        };
    }
    
    public void UnloadMapAssets()
    {
        if (isMapAssetsLoaded && mapAssetsHandle.IsValid())
        {
            Addressables.Release(mapAssetsHandle);
            isMapAssetsLoaded = false;
            Debug.Log("MapAssets unloaded");
        }
    }
    
    public bool IsMenuUILoaded => isMenuUILoaded;
    public bool IsMapAssetsLoaded => isMapAssetsLoaded;
}