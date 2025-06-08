using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("GameObject Settings")]
    public Vector3 initialBallPosition;
    
    [Header("Atlas Configuration")]
    [SerializeField] private AtlasApplierUI uiAtlasApplier;

    [Header("Paddle Settings")]
    [SerializeField] private PaddleController paddleControllerSO;
    [SerializeField] private Transform paddleParent;

    [Header("Ball Settings")]
    public BallController ballControllerSO;
    [SerializeField] private Transform ballPoolContainer;

    [Header("Brick Settings")]
    public BrickController brickControllerSO;
    [SerializeField] private List<Transform> brickPositions;
    [SerializeField] private Transform brickPoolContainer;

    [Header("PowerUp Settings")]
    public PowerUpController powerUpControllerSO;
    [SerializeField] private Transform powerUpPoolContainer;

    [Header("Audio Settings")]
    [SerializeField] private List<AudioClip> bgTracks;
    [SerializeField] private List<AudioSO> soundEffects;
    
    [Header("Console Settings")]
    [SerializeField] private TMP_InputField commandInputField;
    [SerializeField] private List<CommandSO> commands;
    public ConsoleManager consoleManager;
    
    [Header("UI Configuration")]
    [SerializeField] private ButtonSO buttonSO;

    [Header("Layouts UI")]
    public GameObject MainMenuLayout;
    public GameObject PauseLayout;
    public GameObject GameStateLayout;
    [SerializeField] private GameObject consoleUI;
    public GameObject dynamicCanvas;
    
    [Header("Dynamic UI")]
    [SerializeField] private GameCounter[] gameCounters;

    private StateMachine stateMachine = new StateMachine();
    private static bool firstFrame = false;
    private bool bricksSpawned = false;
    private bool ballSpawned = false;
    private static bool initialBallSpawned = false;
    private static bool initialBricksSpawned = false;
    private static PlayerLoopSystem originalPlayerLoop;
    private static bool gameIsReloading = false;

    private struct CustomGameLogic { }

    private void Awake()
    {
        Instance = this;
        InitializeServices();
        firstFrame = false;
        bricksSpawned = false;
        ballSpawned = false;
        initialBallSpawned = false;
        MakePlayerLoop();

        paddleControllerSO.Init(paddleParent);
        brickControllerSO.Init(null);
        InitializeUIManager();
    }
    
    private void InitializeUIManager()
    {
        var uiManager = new UIManager();
        uiManager.Init(dynamicCanvas, gameCounters);
        ServiceProvider.RegisterService(uiManager);
    }

    private void InitializeServices()
    {
        InitializeControllers();
        InitializeAudio();
        InitializePools();
        InitializeButtonManager();
        InitializeUIAtlas();
        InitializeConsole();
    }
    
    private void InitializeConsole()
    {
        var commandManager = new CommandManager();
        commandManager.Init(commands);
        ServiceProvider.RegisterService(commandManager);

        var commandInput = new CommandInput();
        commandInput.Init(commandInputField);
        ServiceProvider.RegisterService(commandInput);

        var consoleManager = new ConsoleManager();
        consoleManager.Init(consoleUI, commandInput);
        ServiceProvider.RegisterService(consoleManager);
    }

    private void InitializeControllers()
    {
        ServiceProvider.RegisterService<PaddleController>(paddleControllerSO);
        ServiceProvider.RegisterService<BallController>(ballControllerSO);
        ServiceProvider.RegisterService<BrickController>(brickControllerSO);
        ServiceProvider.RegisterService<PowerUpController>(powerUpControllerSO);
    }
    
    private void InitializeUIAtlas()
    {
        uiAtlasApplier.ApplyAtlasToLayout(MainMenuLayout);
        uiAtlasApplier.ApplyAtlasToLayout(PauseLayout);
        uiAtlasApplier.ApplyAtlasToLayout(GameStateLayout);
        uiAtlasApplier.ApplyAtlasToLayout(consoleUI);
    }

    private void InitializeAudio()
    {
        var audioManager = new AudioManager(bgTracks, soundEffects);
        var musicSource = gameObject.AddComponent<AudioSource>();
        var sfxSource = gameObject.AddComponent<AudioSource>();
        audioManager.Init(musicSource, sfxSource);
        ServiceProvider.RegisterService(audioManager);
    }

    private void InitializePools()
    {
        var ballPool = new BallPool(ballPoolContainer, ballControllerSO);
        var brickPool = new BrickPool(brickPoolContainer, brickControllerSO);
        var powerUpPool = new PowerUpPool(powerUpPoolContainer, powerUpControllerSO);
        ServiceProvider.RegisterService(ballPool);
        ServiceProvider.RegisterService(brickPool);
        ServiceProvider.RegisterService(powerUpPool);
    }

    private void InitializeButtonManager()
    {
        var buttonManager = new ButtonManager();
        buttonManager.Init(buttonSO);
        ServiceProvider.RegisterService(buttonManager);
    
        ServiceProvider.GetService<ButtonManager>().RegisterButtonsInLayout(MainMenuLayout);
        ServiceProvider.GetService<ButtonManager>().RegisterButtonsInLayout(PauseLayout);
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

        ServiceProvider.GetService<ConsoleManager>().Frame();
        Instance.stateMachine.Tick(Instance);

        if (!initialBricksSpawned)
        {
            initialBricksSpawned = true;
            Instance.SpawnBricksAtPositions();
        }
        
        if (!initialBallSpawned && Instance.IsInGameplayState())
        {
            initialBallSpawned = true;
            BallManager.RespawnSingleBall();
        }

        if (gameIsReloading)
        {
            return;
        }

        Instance.paddleControllerSO.Frame(Time.deltaTime);
        PowerUpManager.Frame();
    }

    private void SpawnBricksAtPositions()
    {
        if (brickPositions == null || brickPositions.Count == 0)
        {
            Debug.LogError("No brick positions assigned!");
            return;
        }

        var brickPool = ServiceProvider.GetService<BrickPool>();
        foreach (Transform position in brickPositions)
        {
            if (position == null) continue;

            var brick = brickPool.SpawnBrick(position.position);
            if (brick != null)
            {
                brick.target.position = position.position;
                brick.Activate();
                BrickManager.Register(brick);
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

        var brickPool = ServiceProvider.GetService<BrickPool>();

        List<BrickController> activeBricks = new List<BrickController>(BrickManager.GetActiveBricks());
        foreach (var brick in activeBricks)
        {
            if (brick != null)
            {
                brick.Reset();
                brickPool.ReturnToPool(brick);
            }
        }

        BrickManager.GetBricks().Clear();
        BrickManager.GetActiveBricks().Clear();

        BallManager.ResetAll();

        PowerUpManager.ResetAll();
        PowerUpManager.ResetPowerUpCount();
        
        ServiceProvider.GetService<UIManager>().ResetCounters();
        Instance.SpawnBricksAtPositions();
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