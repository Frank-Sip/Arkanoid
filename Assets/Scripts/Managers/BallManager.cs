using System.Collections.Generic;
using UnityEngine;

public class BallManager
{
    private static List<BallController> balls = new List<BallController>();
    private static List<BallController> activeBalls = new List<BallController>();
    private static bool hasRespawned = false;
    private static int maxBalls = 5; // Máximo número de pelotas permitidas
    private static bool isMultiballActive = false; // Flag para controlar si estamos en modo multiball

    public static void Register(BallController ball)
    {
        if (!balls.Contains(ball))
        {
            balls.Add(ball);
        }
    }

    public static void SetActive(BallController ball)
    {
        if (!activeBalls.Contains(ball))
        {
            activeBalls.Add(ball);
        }
    }

    public static void Unregister(BallController ball)
    {
        if (activeBalls.Contains(ball))
        {
            activeBalls.Remove(ball);
            
            // Sólo si no quedan bolas activas y no estamos en modo multiball, respawneamos
            if (activeBalls.Count == 0 && !hasRespawned && !isMultiballActive)
            {
                hasRespawned = true;
                Debug.Log("Sin bolas activas, respawneando bola única");
                RespawnSingleBall();
            }
            // Si estamos en modo multiball y se quedaron sin bolas, se reinicia el modo
            else if (activeBalls.Count == 0 && isMultiballActive)
            {
                Debug.Log("Fin del modo multiball, respawneando bola única");
                isMultiballActive = false;
                hasRespawned = true;
                RespawnSingleBall();
            }
        }
    }

    private static void RespawnSingleBall()
    {
        // Usar cualquier bola del pool, no una específica "inicial"
        Vector3 paddlePos = PaddlePhysics.bounds.center;
        Vector3 ballPos = new Vector3(paddlePos.x, paddlePos.y + 3f, 0f);
        
        BallController newBall = BallPool.Instance.SpawnBall(ballPos);
        newBall.SetWaitingOnPaddle();
        
        Debug.Log("Creando nueva bola");
    }

    public static void SpawnMultipleBalls()
    {
        int numberOfBalls = 2;
        int ballsToAdd = Mathf.Min(numberOfBalls, maxBalls - activeBalls.Count);
        
        if (ballsToAdd <= 0)
            return;

        for (int i = 0; i < ballsToAdd; i++)
        {
            Vector3 paddlePos = PaddlePhysics.bounds.center;
            Vector3 ballPos = new Vector3(paddlePos.x + Random.Range(-1f, 1f), paddlePos.y + 3f, 0f);

            BallController newBall = BallPool.Instance.SpawnBall(ballPos);
            newBall.SetWaitingOnPaddle();
        }
    }

    public static void SpawnAndLaunchMultipleBalls(int numberOfBalls = 2)
    {
        // Activar el modo multiball
        isMultiballActive = true;
        
        // Calcular cuántas bolas podemos agregar respetando el límite
        int ballsToAdd = Mathf.Min(numberOfBalls, maxBalls - activeBalls.Count);
        
        // Si ya tenemos el máximo o más, no hacemos nada
        if (ballsToAdd <= 0)
            return;

        // Obtener posición de la paleta
        Vector3 paddlePos = PaddlePhysics.bounds.center;
        Vector3 spawnBasePosition = new Vector3(paddlePos.x, paddlePos.y + 1f, 0f);
        
        // Definir un rango de ángulos más estrecho y centrado para evitar colisiones entre pelotas
        // 70-110 grados, centralizado en 90 (directamente hacia arriba)
        float angleStep = 40f / (ballsToAdd > 1 ? ballsToAdd - 1 : 1);  // 40 grados distribuidos entre las bolas
        float startAngle = 70f;  // Comenzar desde 70 grados (más cerrado hacia arriba)
        
        Debug.Log($"Lanzando {ballsToAdd} bolas, ángulo inicial: {startAngle}, paso: {angleStep}");
        
        // Crear las pelotas con separación temporal para evitar colisiones
        for (int i = 0; i < ballsToAdd; i++)
        {
            // Calcular un pequeño desplazamiento aleatorio para cada bola
            // Usar un desplazamiento más vertical para evitar colisiones iniciales
            Vector3 spawnPosition = spawnBasePosition + new Vector3(Random.Range(-0.2f, 0.2f), 0.3f * i, 0f);
            
            // Calcular el ángulo para esta bola (entre 70 y 110 grados)
            float angle = startAngle + (angleStep * i);
            
            // Calcular la dirección usando el ángulo
            float angleRadians = angle * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Cos(angleRadians), Mathf.Sin(angleRadians), 0);
            
            // Crear y configurar la nueva bola
            BallController newBall = BallPool.Instance.SpawnBall(spawnPosition);
            newBall.Direction = direction.normalized;
            newBall.IsLaunched = true;
            SetActive(newBall);
            
            Debug.Log($"Bola {i} creada con ángulo {angle}, dirección: {direction}");
        }
        
        NotifyBallLaunched();
    }

    public static void NotifyBallLaunched()
    {
        hasRespawned = false;
    }

    public static List<BallController> GetBalls() => balls;
    public static List<BallController> GetActiveBalls() => activeBalls;

    // Método para obtener el máximo número de pelotas
    public static int GetMaxBalls()
    {
        return maxBalls;
    }

    // Método para establecer el máximo número de pelotas
    public static void SetMaxBalls(int max)
    {
        maxBalls = Mathf.Max(1, max); // Siempre al menos 1 bola
    }

    public static void ResetAll()
    {
        // Guardar una copia de la lista para evitar problemas al modificar durante la iteración
        List<BallController> ballsCopy = new List<BallController>(balls);
        
        // Limpiar la lista de bolas activas
        activeBalls.Clear();
        
        // Desactivar todas las bolas (pero mantenerlas registradas)
        foreach (BallController ball in ballsCopy)
        {
            if (ball != null && ball.gameObject.activeInHierarchy)
            {
                // Devolver todas las bolas al pool
                ball.gameObject.SetActive(false);
                BallPool.Instance.ReturnToPool(ball);
            }
        }
        
        // Resetear los flags
        hasRespawned = false;
        isMultiballActive = false;
        
        // Crear una nueva bola inicial
        RespawnSingleBall();
    }
}