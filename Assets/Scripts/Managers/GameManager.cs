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
    public Vector3 initialBallPosition;

    [Header("Paddle Settings")]
    [SerializeField] private PaddleController paddleControllerSO;
    [SerializeField] private Transform paddleParent;

    [Header("Ball Settings")]
    public BallController ballControllerSO;

    [Header("Brick Settings")]
    public BrickController brickControllerSO;
    [SerializeField] private List<Transform> brickParents;

    [Header("PowerUp Settings")]
    public PowerUpController powerUpControllerSO;

    [Header("Brick Grid Settings")]
    [SerializeField] private int columns = 5;
    [SerializeField] private int rows = 5;
    [SerializeField] private float spacing = 0.3f;
    [SerializeField] private Vector2 startPoint = new Vector2(-7f, 4f);
    [SerializeField] private Vector2 endPoint = new Vector2(-7f, 4f);

    [Header("Console Manager")]
    public ConsoleManager consoleManager;

    [Header("Audio Settings")]
    [SerializeField] private List<AudioClip> bgTracks;
    [SerializeField] private List<AudioSO> soundEffects;

    private StateMachine stateMachine = new StateMachine();
    private static bool firstFrame = false;
    private bool bricksSpawned = false;
    private bool ballSpawned = false;
    private static bool initialBallSpawned = false;
    private static bool initialBricksSpawned = false;
    private static PlayerLoopSystem originalPlayerLoop;
    private static bool gameIsReloading = false;

    [Header("Layouts UI")]
    public GameObject MainMenuLayout;
    public GameObject PauseLayout;
    public GameObject GameStateLayout;

    private struct CustomGameLogic { }

    private void Awake()
    {
        Instance = this;
        InitAudioManager();
        firstFrame = false;
        bricksSpawned = false;
        ballSpawned = false;
        initialBallSpawned = false; // Reset this
        MakePlayerLoop();

        // Remove direct initialization of ballControllerSO
        paddleControllerSO.Init(paddleParent);
        brickControllerSO.Init(brickParents[0]);
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

    private void CustomUpdate()
    {
        if (!firstFrame)
        {
            firstFrame = true;
            Instance.stateMachine.ChangeState(new MainMenuState(), Instance);
            return;
        }

        Instance.consoleManager.Frame();
        Instance.stateMachine.Tick(Instance);

        if (!initialBricksSpawned)
        {
            initialBricksSpawned = true;
            Instance.SpawnBricksGrid();
        }

        if (gameIsReloading)
        {
            return;
        }

        Instance.paddleControllerSO.Frame(Time.deltaTime);
        PowerUpManager.Frame();
    }

    private void InitAudioManager()
    {
        var audioManager = new AudioManager(bgTracks, soundEffects);
        var musicSource = gameObject.AddComponent<AudioSource>();
        var sfxSource = gameObject.AddComponent<AudioSource>();
        audioManager.Init(musicSource, sfxSource);

        ServiceProvider.RegisterService(audioManager);
    }

    private void SpawnBricksGrid()
    {
        if (brickParents == null || brickParents.Count == 0)
        {
            Debug.LogError("No brick parents assigned!");
            return;
        }

        BrickSO config = brickControllerSO.brickConfig;
        float width = config.width;
        float height = config.height;

        foreach (var parent in brickParents)
        {
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
                brick.target.transform.position = currentPos;
                brick.target.SetParent(parent);

                BrickManager.Register(brick);

                currentPos.x += width + spacing;
                bricksPlaced++;
            }
        }
    }

    public void ChangeGameStatus(GameState newState)
    {
        stateMachine.ChangeState(newState, this);
    }

    public bool IsInGameplayState()
    {
        return stateMachine.CurrentState is GameplayState;
    }

    public void ResetGame()
    {
        EventManager.ResetGame();
        Instance.paddleControllerSO.Reset();
        Instance.bricksSpawned = false;
        Instance.ballSpawned = false;

        List<BrickController> activeBricks = new List<BrickController>(BrickManager.GetActiveBricks());
        foreach (var brick in activeBricks)
        {
            if (brick != null)
            {
                brick.Reset();
                BrickPool.Instance.ReturnToPool(brick);
            }
        }

        BrickManager.GetBricks().Clear();
        BrickManager.GetActiveBricks().Clear();
        BrickManager.ResetLevelCompletedFlag();

        BallManager.ResetAll();

        PowerUpManager.ResetAll();
        PowerUpManager.ResetPowerUpCount();

        Instance.SpawnBricksGrid();
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