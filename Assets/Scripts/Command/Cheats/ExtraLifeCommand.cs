using UnityEngine;

[CreateAssetMenu(fileName = "ExtraLife", menuName = "Commands/ExtraLife", order = 2)]
public class ExtraLifeCommand : CommandSO
{
    public int livesToAdd = 1;

    public override void Execute()
    {
        PaddleController paddleController = ServiceProvider.GetService<PaddleController>();
        if (paddleController != null)
        {
            paddleController.AddLife(livesToAdd);
        }
    }
}