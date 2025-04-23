using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("GameObject Settings")]
    [SerializeField] private Vector3 initialBallPosition;
    [SerializeField] private PaddleController paddleController;

    [Header("Brick Grid Settings")]
    [SerializeField] private int columns = 5;
    [SerializeField] private int rows = 5;
    [SerializeField] private float spacing = 0.3f;
    [SerializeField] private Vector2 startPoint = new Vector2(-7f, 4f);
    [SerializeField] private Vector2 endPoint = new Vector2(-7f, 4f);

    private StateMachine stateMachine = new StateMachine();
    private static bool firstFrame = false;
    private bool bricksSpawned = false;
    private bool ballSpawned = false;
    private static bool initialBallSpawned = false;
    private static bool initialBricksSpawned = false;
    private static PlayerLoopSystem originalPlayerLoop;
    private static bool gameIsReloading = false;

    [Header ("Layouts UI")]
    public GameObject MainMenuLayout;
    public GameObject PauseLayout;
    public GameObject GameStateLayout;

    private struct CustomGameLogic { }

    private void Awake()
    {
        Instance = this;

        firstFrame = false;
        bricksSpawned = false;
        ballSpawned = false;

        MakePlayerLoop();
        paddleController.Initiate();
    }

    private void MakePlayerLoop()
    {
        originalPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
        var loop = PlayerLoop.GetDefaultPlayerLoop();

        for (int i = 0; i < loop.subSystemList.Length; i++)
        {
            if (loop.subSystemList[i].type == typeof(Update))
            {
                var updateList = new List<PlayerLoopSystem>(loop.subSystemList[i].subSystemList);

                var customSystem = new PlayerLoopSystem
                {
                    type = typeof(CustomGameLogic),
                    updateDelegate = CustomUpdate
                };

                updateList.Insert(0, customSystem);
                loop.subSystemList[i].subSystemList = updateList.ToArray();
                break;
            }
        }

        PlayerLoop.SetPlayerLoop(loop);
    }

    private static void CustomUpdate()
    {
        if (!firstFrame)
        {
            firstFrame = true;
            Instance.stateMachine.ChangeState(new MainMenuState(), Instance);
            return;
        }

        Instance.stateMachine.Update(Instance);

        if (!initialBallSpawned)
        {
            initialBallSpawned = true;
            Debug.Log("Creando bola inicial por primera vez");
            BallController newBall = BallPool.Instance.SpawnBall(Instance.initialBallPosition);
            newBall.SetWaitingOnPaddle();
            newBall.gameObject.SetActive(true);
        }

        if (!initialBricksSpawned)
        {
            initialBricksSpawned = true;
            Instance.SpawnBricksGrid();
        }

        if (gameIsReloading)
        {
            return;
        }

        PaddlePhysics.Frame();
        
        PowerUpManager.Frame();

        foreach (var ball in BallManager.GetBalls())
        {
            if (ball != null && ball.gameObject.activeInHierarchy)
                ball.Frame();
        }
    }

    private void SpawnBricksGrid()
    {
        BrickSO config = BrickPool.Instance.GetBrickSO();
        float width = config.width;
        float height = config.height;

        Vector3 currentPos = startPoint;
        int bricksPlaced = 0;

        for (int i = 0; i < rows * columns; i++)
        {
            if (currentPos.x + width > endPoint.x)
            {
                currentPos.x = startPoint.x;
                currentPos.y -= height + spacing;
            }

            var brick = BrickPool.Instance.SpawnBrick(currentPos);
            brick.transform.position = currentPos;
            
            BrickManager.Register(brick);

            currentPos.x += width + spacing;
            bricksPlaced++;
        }
        
    }

    public void ChangeGameStatus(GameState newState)
    {
        stateMachine.ChangeState(newState, this);
    }

    public void ResetGame()
    {
        EventManager.ResetGame();
        Instance.bricksSpawned = false;
        Instance.ballSpawned = false;
        
        List<BrickController> activeBricks = new List<BrickController>(BrickManager.GetActiveBricks());
        foreach (var brick in activeBricks)
        {
            BrickPool.Instance.ReturnToPool(brick);
        }
        BrickManager.GetBricks().Clear();
        BrickManager.GetActiveBricks().Clear();
        
        BallManager.ResetAll();
        
        PowerUpManager.ResetAll();
        PowerUpManager.ResetPowerUpCount();
        
        Instance.SpawnBricksGrid();
        
        Instance.ChangeGameStatus(new GameplayState());
    }


#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoadMethod]
    private static void ResetPlayerLoopInEditor()
    {
        UnityEditor.EditorApplication.playModeStateChanged += state =>
        {
            if (state == UnityEditor.PlayModeStateChange.ExitingPlayMode)
            {
                PlayerLoop.SetPlayerLoop(originalPlayerLoop);
            }
        };
    }
#endif
}