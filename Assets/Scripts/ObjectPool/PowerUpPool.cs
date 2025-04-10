using UnityEngine;

public class PowerUpPool : MonoBehaviour
{
    [SerializeField] private PowerUpController powerUpPrefab;
    [SerializeField] private int initialPowerUpCount = 5;
    [SerializeField] private Transform poolContainer;
    private int expandPowerUpCount = 5;
    private ObjectPool<PowerUpController> pool;
    public static PowerUpPool Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        pool = new ObjectPool<PowerUpController>(powerUpPrefab, poolContainer, initialPowerUpCount, expandPowerUpCount);
    }

    public PowerUpController SpawnPowerUp(Vector3 position)
    {
        var powerUp = pool.Get();
        powerUp.Activate(position);
        return powerUp;
    }

    public void ReturnToPool(PowerUpController powerUp)
    {
        pool.Return(powerUp);
    }
} 