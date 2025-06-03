using UnityEngine;

public class BrickPool : MonoBehaviour
{
    [SerializeField] private int initialBrickCount = 50;
    [SerializeField] private Transform poolContainer;
    private int expandBrickCount = 10;

    private ObjectPool<BrickController> pool;
    public static BrickPool Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        InitializePool();
    }

    private void InitializePool()
    {
        BrickController brickControllerSO = GameManager.Instance.brickControllerSO;
        
        if (pool == null)
        {
            pool = new ObjectPool<BrickController>(
                brickControllerSO.brickPrefab,
                brickControllerSO,
                poolContainer,
                initialBrickCount,
                expandBrickCount,
                createFunc: () => brickControllerSO.Clone()
            );
        }
    }

    public BrickController SpawnBrick(Vector3 position)
    {
        if (pool == null)
        {
            InitializePool();
        }

        var (controller, instance) = pool.Get();
        if (controller == null || instance == null)
        {
            Debug.LogError("Failed to get brick from pool!");
            return null;
        }

        instance.transform.localScale = Vector3.one;
        instance.transform.position = position;
        instance.transform.SetParent(poolContainer);

        controller.target = instance.transform;
        controller.Init(poolContainer);

        return controller;
    }

    public void ReturnToPool(BrickController controller)
    {
        if (controller == null) return;

        var instance = controller.target?.gameObject;
        if (instance != null)
        {
            instance.SetActive(false);
            instance.transform.localScale = Vector3.one;
            instance.transform.SetParent(poolContainer);
            controller.target = null;
            pool.Return(controller, instance);
        }
    }
}