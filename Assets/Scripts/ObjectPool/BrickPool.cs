using UnityEngine;

public class BrickPool : MonoBehaviour
{
    [SerializeField] private BrickController brickPrefab;
    [SerializeField] private int initialBrickCount = 100;
    [SerializeField] private Transform poolContainer;
    private int expandBrickCount = 10;
    private ObjectPool<BrickController> pool;
    public static BrickPool Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        pool = new ObjectPool<BrickController>(brickPrefab, poolContainer, initialBrickCount, expandBrickCount);
    }

    public BrickController SpawnBrick(Vector3 position)
    {
        var brick = pool.Get();
        brick.transform.position = position;
        BrickManager.Register(brick);
        brick.Activate();
        
        return brick;
    }

    public void ReturnToPool(BrickController brick)
    {
        BrickManager.Unregister(brick);
        pool.Return(brick);
        brick.gameObject.SetActive(false);
    }

    public BrickSO GetBrickSO()
    {
        return brickPrefab.brickConfig;
    }
}