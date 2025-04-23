using System.Collections.Generic;
using UnityEngine;

public static class BrickManager
{
    private static readonly List<BrickController> bricks = new List<BrickController>();
    private static readonly List<BrickController> activeBricks = new List<BrickController>();

    public static void Register(BrickController brick)
    {
        if (!bricks.Contains(brick)) bricks.Add(brick);
        if (!activeBricks.Contains(brick)) activeBricks.Add(brick);
    }

    public static void Unregister(BrickController brick)
    {
        activeBricks.Remove(brick);

        if (activeBricks.Count <= 0)
        {
            GameManager.Instance.ResetGame();
        }
    }

    public static List<BrickController> GetBricks() => bricks;

    public static List<BrickController> GetActiveBricks() => activeBricks;
}
