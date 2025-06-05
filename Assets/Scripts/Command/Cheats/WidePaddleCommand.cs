using UnityEngine;

[CreateAssetMenu(fileName = "WidePaddle", menuName = "Commands/WidePaddle", order = 1)]
public class WidePaddleCommand : CommandSO
{
    public float scaleMultiplier = 1.5f;
    public float duration = 5f;

    public override void Execute()
    {
        PaddleController paddleController = Object.FindObjectOfType<PaddleController>();
        if (paddleController != null)
        {
            paddleController.ActivateWidePaddle(scaleMultiplier, duration);
        }
    }
}