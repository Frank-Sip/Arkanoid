using UnityEngine;
using System.Collections.Generic;

public static class BrickManager
{
    private static readonly List<BrickController> bricks = new List<BrickController>();
    private static readonly List<BrickController> activeBricks = new List<BrickController>();

    public static void Register(BrickController brick)
    {
        if (!bricks.Contains(brick))
        {
            bricks.Add(brick);
            activeBricks.Add(brick);
        }
    }

    public static void Unregister(BrickController brick)
    {
        activeBricks.Remove(brick);
        CheckGameCondition();
    }
    
    public static void SpawnBricksAtPositions()
    {
        SpawnBricksOfType(BrickType.Weak);
        SpawnBricksOfType(BrickType.Normal);
        SpawnBricksOfType(BrickType.Strong);
        SpawnBricksOfType(BrickType.Tough);
    }
    
    private static void SpawnBricksOfType(BrickType type)
    {
        string tag = type.ToString();
        GameObject brickParent = GameObject.FindWithTag(tag);
        if (brickParent == null) return;

        var brickPool = ServiceProvider.GetService<BrickPool>();
        Transform parentTransform = brickParent.transform;

        for (int i = 0; i < parentTransform.childCount; i++)
        {
            Transform childPosition = parentTransform.GetChild(i);
            var brick = brickPool.SpawnBrick(childPosition.position);

            if (brick != null)
            {
                brick.target.position = childPosition.position;
                brick.SetBrickType(type);
                brick.Activate();
                Register(brick);
            }
        }
    }    private static void CheckGameCondition()
    {
        if (activeBricks.Count <= 0 && GameManager.Instance.IsInGameplayState())
        {
            // Level completed - notify LevelManager
            LevelManager.OnLevelCompleted();
        }
    }

    public static List<BrickController> GetBricks() => bricks;

    public static List<BrickController> GetActiveBricks() => activeBricks;    public static void ClearAllBricks()
    {
        var brickPool = ServiceProvider.GetService<BrickPool>();
        
        // Return all bricks to pool
        for (int i = bricks.Count - 1; i >= 0; i--)
        {
            var brick = bricks[i];
            if (brick != null)
            {
                brick.Reset();
                brickPool.ReturnToPool(brick);
            }
        }
        
        // Clear the lists
        bricks.Clear();
        activeBricks.Clear();
    }

    public static int GetActiveBrickCount()
    {
        return activeBricks.Count;
    }
}