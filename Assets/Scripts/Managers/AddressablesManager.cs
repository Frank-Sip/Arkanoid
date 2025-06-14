using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AddressablesManager
{
    private static Dictionary<string, AsyncOperationHandle> loadedGroups = new Dictionary<string, AsyncOperationHandle>();

    public static void LoadGroup(string groupName, System.Action<bool> onComplete = null)
    {
        if (loadedGroups.ContainsKey(groupName))
        {
            onComplete?.Invoke(true);
            return;
        }

        var handle = Addressables.LoadResourceLocationsAsync(groupName);
        handle.Completed += (locationsHandle) =>
        {
            if (locationsHandle.Status == AsyncOperationStatus.Succeeded)
            {
                var assetsHandle = Addressables.LoadAssetsAsync<object>(groupName, null);
                assetsHandle.Completed += (loadHandle) =>
                {
                    if (loadHandle.Status == AsyncOperationStatus.Succeeded)
                    {
                        loadedGroups[groupName] = loadHandle;
                        onComplete?.Invoke(true);
                    }
                    else
                    {
                        Debug.LogError($"Failed to load addressable group: {groupName}");
                        onComplete?.Invoke(false);
                    }
                };
            }
            else
            {
                Debug.LogError($"Failed to load resource locations for group: {groupName}");
                onComplete?.Invoke(false);
            }
        };
    }

    public static void UnloadGroup(string groupName)
    {
        if (loadedGroups.TryGetValue(groupName, out var handle))
        {
            Addressables.Release(handle);
            loadedGroups.Remove(groupName);
            Debug.Log($"Unloaded addressable group: {groupName}");
        }
    }

    public static bool IsGroupLoaded(string groupName)
    {
        return loadedGroups.ContainsKey(groupName);
    }

    public static void UnloadAllGroups()
    {
        foreach (var kvp in loadedGroups)
        {
            Addressables.Release(kvp.Value);
        }
        loadedGroups.Clear();
    }
}
