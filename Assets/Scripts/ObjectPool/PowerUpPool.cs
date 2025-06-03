using System.Collections.Generic;
using UnityEngine;

public class PowerUpPool : MonoBehaviour
{
    [SerializeField] private int initialPowerUpCount = 10;
    [SerializeField] private Transform poolContainer;
    private int expandPowerUpCount = 5;

    private ObjectPool<PowerUpController> pool;
    public static PowerUpPool Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        InitializePool();
    }

    private void InitializePool()
    {
        PowerUpController powerUpControllerSO = GameManager.Instance.powerUpControllerSO;

        if (pool == null)
        {
            pool = new ObjectPool<PowerUpController>(
                powerUpControllerSO.powerUpPrefab,
                powerUpControllerSO,
                poolContainer,
                initialPowerUpCount,
                expandPowerUpCount,
                createFunc: () => powerUpControllerSO.Clone()
            );
        }
    }

    public PowerUpController SpawnPowerUp(Vector3 position)
    {
        if (pool == null)
        {
            InitializePool();
        }

        var (controller, instance) = pool.Get();

        instance.transform.localScale = Vector3.one;
        instance.transform.position = position;
        instance.transform.SetParent(poolContainer);

        controller.target = instance.transform;
        controller.Init();
        controller.Activate();

        return controller;
    }

    public void ReturnToPool(PowerUpController controller)
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

    public void ClearPool()
    {
        if (pool != null)
        {
            var powerUps = PowerUpManager.GetPowerUps();
            foreach (var powerUp in powerUps)
            {
                if (powerUp != null && powerUp.target != null)
                {
                    powerUp.target.gameObject.SetActive(false);
                    ReturnToPool(powerUp);
                }
            }
        }
    }
}