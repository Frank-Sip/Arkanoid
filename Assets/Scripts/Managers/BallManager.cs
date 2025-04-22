using System.Collections.Generic;
using UnityEngine;

public class BallManager
{
    private static List<BallController> balls = new List<BallController>();
    private static List<BallController> activeBalls = new List<BallController>();
    private static bool hasRespawned = false; // NUEVO
    private static int maxBalls = 5; // Máximo número de pelotas permitidas

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
            
            if (ball == BallPool.Instance.GetInitialBall() && activeBalls.Count == 0 && !hasRespawned)
            {
                hasRespawned = true;
                Debug.Log("Bola inicial perdida, respawneando");
                RespawnSingleBall();
            }
            else if (activeBalls.Count == 0 && !hasRespawned)
            {
                hasRespawned = true;
                Debug.Log("Sin bolas activas, respawneando bola única");
                RespawnSingleBall();
            }
        }
    }

    private static void RespawnSingleBall()
    {
        BallController initialBall = BallPool.Instance.GetInitialBall();
        
        if (initialBall != null)
        {
            initialBall.gameObject.SetActive(true);
            initialBall.SetWaitingOnPaddle();
            
            Vector3 paddlePos = PaddlePhysics.bounds.center;
            initialBall.transform.position = new Vector3(paddlePos.x, paddlePos.y + 3f, 0f);
            
            Debug.Log("Reposicionando bola inicial");
        }
        else
        {
            Vector3 paddlePos = PaddlePhysics.bounds.center;
            Vector3 ballPos = new Vector3(paddlePos.x, paddlePos.y + 3f, 0f);
            
            BallController newBall = BallPool.Instance.SpawnBall(ballPos);
            newBall.SetWaitingOnPaddle();
            
            Debug.Log("Creando nueva bola inicial");
        }
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
        // Calcular cuántas bolas podemos agregar respetando el límite
        int ballsToAdd = Mathf.Min(numberOfBalls, maxBalls - activeBalls.Count);
        
        // Si ya tenemos el máximo o más, no hacemos nada
        if (ballsToAdd <= 0)
            return;

        for (int i = 0; i < ballsToAdd; i++)
        {
            // Usar la posición de una bola activa como referencia si existe
            Vector3 spawnPosition;
            Vector3 direction;
            
            if (activeBalls.Count > 0 && activeBalls[0] != null)
            {
                // Crear bola cerca de una bola existente con un pequeño desplazamiento
                BallController referenceBall = activeBalls[0];
                spawnPosition = referenceBall.transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0f);
                direction = referenceBall.Direction;
                
                // Rotamos ligeramente la dirección para que las bolas no vayan exactamente igual
                float rotationAngle = Random.Range(-30f, 30f);
                direction = Quaternion.Euler(0, 0, rotationAngle) * direction;
            }
            else
            {
                // Si no hay bolas activas, crear sobre la paleta
                Vector3 paddlePos = PaddlePhysics.bounds.center;
                spawnPosition = new Vector3(paddlePos.x, paddlePos.y + 1f, 0f);
                direction = BallPhysics.GetInitialDirection();
            }
            
            BallController newBall = BallPool.Instance.SpawnBall(spawnPosition);
            newBall.Direction = direction;
            newBall.IsLaunched = true;
            SetActive(newBall);
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
                if (ball == BallPool.Instance.GetInitialBall())
                {
                    // Para la bola inicial, solo reiniciarla
                    ball.SetWaitingOnPaddle();
                    ball.gameObject.SetActive(true);
                }
                else
                {
                    // Para las demás bolas, devolverlas al pool
                    BallPool.Instance.ReturnToPool(ball);
                }
            }
        }
        
        // Resetear el flag
        hasRespawned = false;
    }
}