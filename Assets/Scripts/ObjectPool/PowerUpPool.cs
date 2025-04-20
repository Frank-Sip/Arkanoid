using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpPool : MonoBehaviour
{
    [SerializeField] private PowerUpController powerUpPrefab;
    [SerializeField] private int initialPowerUpCount = 10;
    [SerializeField] private Transform poolContainer;

    private ObjectPool<PowerUpController> pool;
    public static PowerUpPool Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        pool = new ObjectPool<PowerUpController>(powerUpPrefab, poolContainer, initialPowerUpCount);
    }

    public PowerUpController SpawnPowerUp(Vector3 position)
    {
        var powerUp = pool.Get();
        powerUp.transform.position = position;
        powerUp.gameObject.SetActive(true);
        return powerUp;
    }

    public void ReturnToPool(PowerUpController powerUp)
    {
        powerUp.gameObject.SetActive(false);
        pool.Return(powerUp);
    }
}