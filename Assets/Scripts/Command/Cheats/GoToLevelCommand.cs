using UnityEngine;

[CreateAssetMenu(fileName = "GoToLevel", menuName = "Commands/GoToLevel", order = 0)]
public class GoToLevelCommand : CommandSO
{
    [SerializeField] private int targetLevel = 1;

    public override void Execute()
    {
        if (!GameManager.Instance.IsInGameplayState())
        {
            Debug.Log("Comando GoToLevel solo puede ser usado durante el gameplay.");
            return;
        }

        if (LevelManager.IsTransitioning)
        {
            Debug.Log("El nivel ya está cambiando. Espera un momento.");
            return;
        }

        if (targetLevel < 1 || targetLevel > LevelManager.GetTotalLevels())
        {
            Debug.Log($"Nivel inválido. Niveles disponibles: 1-{LevelManager.GetTotalLevels()}");
            return;
        }

        Debug.Log($"Yendo al nivel {targetLevel}...");
        LevelManager.GoToLevel(targetLevel - 1); // Convert to 0-based index
    }

    public void SetTargetLevel(int level)
    {
        targetLevel = level;
    }
}
