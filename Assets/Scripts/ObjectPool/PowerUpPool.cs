using System.Collections.Generic;
using UnityEngine;

public class PowerUpPool : MonoBehaviour
{
    [SerializeField] private int initialPowerUpCount = 10;
    [SerializeField] private Transform poolContainer;
    private int expandPowerUpCount = 10;

    private ObjectPool<PowerUpController> pool;
    public static PowerUpPool Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public PowerUpController SpawnPowerUp(Vector3 position)
    {
        PowerUpController powerUpControllerSO = GameManager.Instance.powerUpControllerSO;
        
        if (powerUpControllerSO.target == null)
        {
            powerUpControllerSO.Init(poolContainer);
        }

        var (controller, instance) = pool.Get();
        instance.transform.position = position;
        controller.Init(instance.transform);
        return controller;
    }

    public void ReturnToPool(PowerUpController controller)
    {
        if (controller == null) return;
        pool.Return(controller, controller.target.gameObject);
    }
}