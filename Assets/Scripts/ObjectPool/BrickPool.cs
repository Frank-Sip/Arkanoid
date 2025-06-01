using UnityEngine;

public class BrickPool : MonoBehaviour
{
    [SerializeField] private int initialBrickCount = 50;
    [SerializeField] private Transform poolContainer;
    private int expandBrickCount = 20;

    private ObjectPool<BrickController> pool;
    public static BrickPool Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        
        BrickController brickControllerSO = GameManager.Instance.brickControllerSO;

        pool = new ObjectPool<BrickController>(
            brickControllerSO.target.gameObject,
            brickControllerSO,
            poolContainer,
            initialBrickCount,
            expandBrickCount
        );
    }

    public BrickController SpawnBrick(Vector3 position)
    {
        var (controller, instance) = pool.Get();
        instance.transform.position = position;
        controller.Init(instance.transform);
        return controller;
    }

    public void ReturnToPool(BrickController controller)
    {
        if (controller == null) return;
        pool.Return(controller, controller.target.gameObject);
    }
}