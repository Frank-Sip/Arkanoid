using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpPool : MonoBehaviour
{
    [SerializeField] private PowerUpController powerUpPrefab;
    [SerializeField] private int initialPowerUpCount = 3;
    [SerializeField] private Transform poolContainer;
    private int expandPowerUpCount = 3;
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
        powerUp.transform.position = position;
        PowerUpManager.Register(powerUp);
        return powerUp;
    }

    public void ReturnToPool(PowerUpController powerUp)
    {
        PowerUpManager.Unregister(powerUp);
        pool.Return(powerUp);
    }
}