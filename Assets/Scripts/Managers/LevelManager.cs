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
    }

    private void SetupLevel(LevelData levelData)
    {
        CleanupCurrentLevel();
        
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
                deactivatedLevels.Add(instance);
            }
        }
        currentLevelInstances.Clear();

        GameManager.Instance.ResetGame();
    }

    private void HandleLevelCompleted()
    {
        currentLevel++;

        if (currentLevel > totalLevels)
        {
            GameManager.Instance.ChangeGameStatus(new VictoryState());
            return;
        }

        if (currentLevel == 6)
        {
            addressableManager.UnloadPackage("LevelPack1");
        }

        LoadCurrentLevel();
    }
}