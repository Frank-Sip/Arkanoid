using System.Collections.Generic;

public static class BrickManager
{
    private static readonly List<BrickController> bricks = new List<BrickController>();
    private static readonly List<BrickController> activeBricks = new List<BrickController>();
    private static bool levelCompleted = false;

    public static void Register(BrickController brick)
    {
        if (!bricks.Contains(brick)) bricks.Add(brick);
        if (!activeBricks.Contains(brick)) activeBricks.Add(brick);
    }

    public static void Unregister(BrickController brick)
    {
        activeBricks.Remove(brick);

        if (activeBricks.Count <= 0 && !levelCompleted && GameManager.Instance.IsInGameplayState())
        {
            levelCompleted = true;
            GameManager.Instance.ResetGame();
            GameManager.Instance.ChangeGameStatus(new MainMenuState());
        }
    }

    public static void ResetLevelCompletedFlag()
    {
        levelCompleted = false;
    }

    public static List<BrickController> GetBricks() => bricks;

    public static List<BrickController> GetActiveBricks() => activeBricks;
}