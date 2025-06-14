using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableManager
{
    private Dictionary<int, string> levelKeyMap = new Dictionary<int, string>();
    private Dictionary<int, string> levelToPackMap = new Dictionary<int, string>();
    private HashSet<string> loadedPacks = new HashSet<string>();

    public void Init()
    {
        for (int i = 1; i <= 10; i++)
        {
            levelKeyMap[i] = $"Level{i}";
            levelToPackMap[i] = i <= 5 ? "LevelPack1" : "LevelPack2";
        }
    }

    public void LoadLevelAsync(int levelNumber, System.Action<LevelData> onComplete)
    {
        string levelKey = levelKeyMap[levelNumber];
        string packKey = levelToPackMap[levelNumber];
        
        if (!loadedPacks.Contains(packKey))
        {
            Addressables.LoadAssetsAsync<UnityEngine.Object>(packKey, null).Completed += handle => {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    loadedPacks.Add(packKey);
                    LoadLevelData(levelKey, onComplete);
                }
            };
        }
        else
        {
            LoadLevelData(levelKey, onComplete);
        }
    }

    private void LoadLevelData(string levelKey, System.Action<LevelData> onComplete)
    {
        Addressables.LoadAssetAsync<LevelData>(levelKey).Completed += handle => {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                onComplete?.Invoke(handle.Result);
            }
        };
    }

    public void UnloadPackage(string packKey)
    {
        if (loadedPacks.Contains(packKey))
        {
            Addressables.Release(packKey);
            loadedPacks.Remove(packKey);
        }
    }
}