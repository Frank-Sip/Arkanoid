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
        // Buscar el objeto padre con el tag "BrickPositions"
        GameObject brickParent = GameObject.FindWithTag("Weak");

        var brickPool = ServiceProvider.GetService<BrickPool>();
        Transform parentTransform = brickParent.transform;

        // Recorrer todos los hijos y crear bricks en sus posiciones
        for (int i = 0; i < parentTransform.childCount; i++)
        {
            Transform childPosition = parentTransform.GetChild(i);
            var brick = brickPool.SpawnBrick(childPosition.position);
        
            if (brick != null)
            {
                brick.target.position = childPosition.position;
                brick.Activate();
                BrickManager.Register(brick);
            }
        }
    }

    private static void CheckGameCondition()
    {
        if (activeBricks.Count <= 0 && GameManager.Instance.IsInGameplayState())
        {
            GameManager.Instance.ResetGame();
            GameManager.Instance.ChangeGameStatus(new MainMenuState());
        }
    }

    public static List<BrickController> GetBricks() => bricks;

    public static List<BrickController> GetActiveBricks() => activeBricks;
}