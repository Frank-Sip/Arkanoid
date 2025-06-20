using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager
{
    private AddressableManager addressableManager;
    private int currentLevel = 1;
    private int totalLevels = 10;
    private bool isLoading = false;
    private List<GameObject> currentLevelInstances = new List<GameObject>();
    private List<GameObject> deactivatedLevels = new List<GameObject>();

    public void Init(AddressableManager addressableManager, int totalLevels)
    {
        this.addressableManager = addressableManager;
        this.totalLevels = totalLevels;
        BrickManager.OnAllBricksDestroyed += HandleLevelCompleted;
    }

    public void StartGame()
    {
        currentLevel = 1;
        LoadCurrentLevel();
    }

    public void LoadCurrentLevel()
    {
        if (isLoading) return;
        isLoading = true;

        addressableManager.LoadLevelAsync(currentLevel, levelData => {
            if (levelData != null)
            {
                SetupLevel(levelData);
            }
            isLoading = false;
        });
    }    private void SetupLevel(LevelData levelData)
    {
        CleanupCurrentLevel();
        UnityEngine.Object.FindObjectOfType<MonoBehaviour>().StartCoroutine(SetupLevelAfterCleanup(levelData));
    }

    private System.Collections.IEnumerator SetupLevelAfterCleanup(LevelData levelData)
    {
        yield return null;
        
        foreach (var prefab in levelData.levelPrefabs)
        {
            if (prefab != null)
            {
                GameObject instance = GameObject.Instantiate(prefab);
                currentLevelInstances.Add(instance);
            }
        }

        BrickManager.SpawnBricksAtPositions();
    }    
      private void CleanupCurrentLevel()
    {
        foreach (var instance in currentLevelInstances)
        {
            if (instance != null)
            {
                instance.SetActive(false);
                GameObject.DestroyImmediate(instance);
            }
        }
        currentLevelInstances.Clear();

        foreach (var deactivated in deactivatedLevels)
        {
            if (deactivated != null)
            {
                GameObject.DestroyImmediate(deactivated);
            }
        }
        deactivatedLevels.Clear();

        GameManager.Instance.ResetGame();
        
     
    }    private void HandleLevelCompleted()
    {
        currentLevel++;

        if (currentLevel > totalLevels)
        {
            GameManager.Instance.ChangeGameStatus(new VictoryState());
            return;
        }

        if (currentLevel == 6)
        {
            Debug.Log("Transitioning from LevelPack1 to LevelPack2 - Unloading LevelPack1");
            
            CleanupCurrentLevel();
            
            addressableManager.UnloadPackage("LevelPack1");
            
            UnityEngine.Object.FindObjectOfType<MonoBehaviour>().StartCoroutine(LoadNextLevelAfterPackUnload());
        }
        else
        {
            LoadCurrentLevel();
        }
    }
    
    private System.Collections.IEnumerator LoadNextLevelAfterPackUnload()
    {
        yield return null;
        yield return null;
        
        Debug.Log($"Loading Level {currentLevel} from LevelPack2");
        LoadCurrentLevel();
    }
}