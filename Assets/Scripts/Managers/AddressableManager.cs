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

    public void Init()
    {
        for (int i = 1; i <= 10; i++)
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
            // Cargar el grupo/paquete completo
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
            // El paquete ya est� cargado
            LoadLevelData(levelKey, onComplete, onError);
        }
    }

    private void LoadLevelData(string levelKey, System.Action<LevelData> onComplete, System.Action<string> onError)
    {
        var levelHandle = Addressables.LoadAssetAsync<LevelData>(levelKey);

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
    }

    public void UnloadPackage(string packKey)
    {
        if (loadedPackHandles.ContainsKey(packKey))
        {
            // Liberar correctamente usando el handle
            Addressables.Release(loadedPackHandles[packKey]);
            loadedPackHandles.Remove(packKey);
            Debug.Log($"Pack {packKey} unloaded");
        }
    }

    public void UnloadAllPackages()
    {
        foreach (var kvp in loadedPackHandles)
        {
            Addressables.Release(kvp.Value);
        }
        loadedPackHandles.Clear();
        Debug.Log("All packs unloaded");
    }

    public bool IsPackLoaded(string packKey)
    {
        return loadedPackHandles.ContainsKey(packKey);
    }

    public void Dispose()
    {
        UnloadAllPackages();
    }
}