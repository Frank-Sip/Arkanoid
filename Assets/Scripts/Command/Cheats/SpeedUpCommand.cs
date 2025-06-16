using UnityEngine;

[CreateAssetMenu(fileName = "SpeedUp", menuName = "Commands/SpeedUp", order = 3)]
public class SpeedUpCommand : CommandSO
{
    public float speedMultiplier = 1.5f;
    public float duration = 5f;

    public override void Execute()
    {
        PaddleController paddleController = ServiceProvider.GetService<PaddleController>();
        if (paddleController != null)
        {
            paddleController.ActivateSpeedBoost(speedMultiplier, duration);
        }
    }
}