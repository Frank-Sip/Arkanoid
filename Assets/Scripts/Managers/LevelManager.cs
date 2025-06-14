using System.Collections.Generic;
using UnityEngine;

public static class LevelManager
{
    private static List<string> levelAddressables = new List<string>();
    private static int currentLevel = 0;
    private static bool isTransitioning = false;

    public static int CurrentLevel => currentLevel + 1; // 1-based for display
    public static int CurrentRound => currentLevel; // 0-based for internal use
    public static bool IsTransitioning => isTransitioning;    public static void Initialize(List<string> addressableNames)
    {
        levelAddressables = new List<string>(addressableNames);
        currentLevel = 0;
        
        // If no addressables are configured, just spawn bricks directly
        if (levelAddressables.Count == 0 || string.IsNullOrEmpty(levelAddressables[0]))
        {
            Debug.Log("No addressables configured, spawning bricks directly");
            BuildLevelBricks();
            return;
        }
        
        // Load the first level
        LoadCurrentLevel();
    }

    public static void OnLevelCompleted()
    {
        if (isTransitioning) return;

        Debug.Log($"Level {CurrentLevel} completed! Moving to next level...");
        
        // Check if there are more levels
        if (currentLevel + 1 < levelAddressables.Count)
        {
            TransitionToNextLevel();
        }
        else
        {
            // All levels completed - could trigger victory state
            Debug.Log("All levels completed! Game finished!");
            OnAllLevelsCompleted();
        }
    }

    private static void TransitionToNextLevel()
    {
        isTransitioning = true;
        
        // Unload current level
        string currentLevelName = levelAddressables[currentLevel];
        AddressablesManager.UnloadGroup(currentLevelName);
        
        // Move to next level
        currentLevel++;
        
        // Load next level
        LoadCurrentLevel();
    }    private static void LoadCurrentLevel()
    {
        if (currentLevel >= levelAddressables.Count)
        {
            Debug.LogError("Trying to load level beyond available levels!");
            return;
        }

        string levelName = levelAddressables[currentLevel];
        
        // If level name is empty or null, just build bricks directly
        if (string.IsNullOrEmpty(levelName))
        {
            BuildLevelBricks();
            isTransitioning = false;
            return;
        }

        AddressablesManager.LoadGroup(levelName, (success) =>
        {
            if (success)
            {
                BuildLevelBricks();
                isTransitioning = false;
            }
            else
            {
                Debug.LogError($"Failed to load level {CurrentLevel}: {levelName}. Building bricks directly.");
                BuildLevelBricks();
                isTransitioning = false;
            }
        });
    }    private static void BuildLevelBricks()
    {
        // Clear existing bricks first
        ClearActiveBricks();
        
        // Spawn new bricks based on tags in the scene
        BrickManager.SpawnBricksAtPositions();
    }private static void ClearActiveBricks()
    {
        // Use the new method in BrickManager to clear all bricks
        BrickManager.ClearAllBricks();
    }

    private static void OnAllLevelsCompleted()
    {
        // Reset to first level or trigger victory state
        Debug.Log("All levels completed! Resetting to level 1...");
        currentLevel = 0;
        LoadCurrentLevel();
        
        // Could also trigger a victory state here
        // GameManager.Instance.ChangeGameStatus(new VictoryState());
    }

    public static void RestartCurrentLevel()
    {
        if (isTransitioning) return;

        isTransitioning = true;
        Debug.Log($"Restarting level {CurrentLevel}");
        
        // Reload current level
        string currentLevelName = levelAddressables[currentLevel];
        AddressablesManager.UnloadGroup(currentLevelName);
        
        LoadCurrentLevel();
    }

    public static void GoToLevel(int levelIndex)
    {
        if (isTransitioning) return;
        
        if (levelIndex < 0 || levelIndex >= levelAddressables.Count)
        {
            Debug.LogError($"Invalid level index: {levelIndex}. Available levels: 0-{levelAddressables.Count - 1}");
            return;
        }

        isTransitioning = true;
        
        // Unload current level
        string currentLevelName = levelAddressables[currentLevel];
        AddressablesManager.UnloadGroup(currentLevelName);
        
        // Set new level
        currentLevel = levelIndex;
        
        // Load target level
        LoadCurrentLevel();
    }

    public static string GetCurrentLevelName()
    {
        if (currentLevel >= 0 && currentLevel < levelAddressables.Count)
        {
            return levelAddressables[currentLevel];
        }
        return "Unknown";
    }

    public static int GetTotalLevels()
    {
        return levelAddressables.Count;
    }

    public static void ForceCompleteLevel()
    {
        Debug.Log("Force completing current level...");
        OnLevelCompleted();
    }

    public static void Cleanup()
    {
        // Unload current level when cleaning up
        if (currentLevel >= 0 && currentLevel < levelAddressables.Count)
        {
            string currentLevelName = levelAddressables[currentLevel];
            AddressablesManager.UnloadGroup(currentLevelName);
        }
        
        levelAddressables.Clear();
        currentLevel = 0;
        isTransitioning = false;
    }
}
