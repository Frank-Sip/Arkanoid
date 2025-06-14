using UnityEngine;

[CreateAssetMenu(fileName = "RestartLevel", menuName = "Commands/RestartLevel", order = 0)]
public class RestartLevelCommand : CommandSO
{
    public override void Execute()
    {
        if (!GameManager.Instance.IsInGameplayState())
        {
            Debug.Log("Comando RestartLevel solo puede ser usado durante el gameplay.");
            return;
        }

        if (LevelManager.IsTransitioning)
        {
            Debug.Log("El nivel ya está cambiando. Espera un momento.");
            return;
        }

        Debug.Log($"Reiniciando nivel {LevelManager.CurrentLevel}...");
        LevelManager.RestartCurrentLevel();
    }
}
