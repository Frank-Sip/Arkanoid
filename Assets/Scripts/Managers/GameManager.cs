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

    private StateMachine stateMachine = new StateMachine();
    private static bool firstFrame = false;
    private static bool initialBallSpawned = false;
    private static bool initialBricksSpawned = false;
    private static PlayerLoopSystem originalPlayerLoop;
    private static bool gameIsReloading = false;

    private void Awake()
    {
        Instance = this;
        
        firstFrame = false;
        initialBallSpawned = false;
        initialBricksSpawned = false;
        gameIsReloading = false;

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
        //Needed because the game freezes in the first frame
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
            BallPool.Instance.SpawnBall(Instance.initialBallPosition);
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

        foreach (var ball in BallManager.GetBalls())
        {
            if (ball.gameObject.activeInHierarchy)
                ball.Frame();
        }
    }

    private void SpawnBricksGrid()
    {
        BrickSO config = BrickPool.Instance.GetBrickSO();
        float width = config.width;
        float height = config.height;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 pos = new Vector3(
                    startPoint.x + col * (width + spacing),
                    startPoint.y - row * (height + spacing),
                    0f
                );

                BrickController brick = BrickPool.Instance.SpawnBrick();
                brick.transform.position = pos;
                brick.Activate();
            }
        }
    }
    
    public void ChangeGameStatus(GameState newState)
    {
        stateMachine.ChangeState(newState, this);
    }
    
    public static void ReloadGame()
    {
        gameIsReloading = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    private struct CustomGameLogic { }

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