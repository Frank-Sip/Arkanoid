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