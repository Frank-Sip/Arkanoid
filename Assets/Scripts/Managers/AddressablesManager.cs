using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AddressablesManager
{
    private static Dictionary<string, AsyncOperationHandle> loadedGroups = new Dictionary<string, AsyncOperationHandle>();

    public static async Task LoadGroupAsync(string groupName)
    {
        if (loadedGroups.ContainsKey(groupName))
            return;
        var handle = Addressables.LoadResourceLocationsAsync(groupName);
        await handle.Task;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            var assetsHandle = Addressables.LoadAssetsAsync<object>(groupName, null);
            await assetsHandle.Task;
            loadedGroups[groupName] = assetsHandle;
        }
    }

    public static async Task UnloadGroupAsync(string groupName)
    {
        if (loadedGroups.TryGetValue(groupName, out var handle))
        {
            Addressables.Release(handle);
            loadedGroups.Remove(groupName);
        }
    }

    public static bool IsGroupLoaded(string groupName)
    {
        return loadedGroups.ContainsKey(groupName);
    }
}
