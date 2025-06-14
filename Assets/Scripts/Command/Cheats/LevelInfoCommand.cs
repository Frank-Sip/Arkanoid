using UnityEngine;

[CreateAssetMenu(fileName = "LevelInfo", menuName = "Commands/LevelInfo", order = 0)]
public class LevelInfoCommand : CommandSO
{
    public override void Execute()
    {
        Debug.Log($"=== INFORMACIÓN DEL NIVEL ===");
        Debug.Log($"Nivel Actual: {LevelManager.CurrentLevel}");
        Debug.Log($"Ronda Actual: {LevelManager.CurrentRound}");
        Debug.Log($"Nombre del Nivel: {LevelManager.GetCurrentLevelName()}");
        Debug.Log($"Total de Niveles: {LevelManager.GetTotalLevels()}");
        Debug.Log($"Ladrillos Activos: {BrickManager.GetActiveBrickCount()}");
        Debug.Log($"En Transición: {(LevelManager.IsTransitioning ? "Sí" : "No")}");
        Debug.Log($"Estado del Juego: {(GameManager.Instance.IsInGameplayState() ? "Gameplay" : "Otro")}");
    }
}
