using UnityEngine;

[CreateAssetMenu(fileName = "NextLevel", menuName = "Commands/NextLevel", order = 0)]
public class NextLevelCommand : CommandSO
{
    public override void Execute()
    {
        if (!GameManager.Instance.IsInGameplayState())
        {
            Debug.Log("Comando NextLevel solo puede ser usado durante el gameplay.");
            return;
        }

        if (LevelManager.IsTransitioning)
        {
            Debug.Log("El nivel ya está cambiando. Espera un momento.");
            return;
        }

        Debug.Log($"Forzando completar nivel {LevelManager.CurrentLevel}...");
        LevelManager.ForceCompleteLevel();
    }
}
