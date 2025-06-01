using UnityEngine;

[CreateAssetMenu(fileName = "PowerUpController", menuName = "GameObject/PowerUpController")]
public class PowerUpController : ScriptableObject
{
    public PowerUpSO powerUpSO;
    [SerializeField] private ScreenEdgesSO screenEdgesSO;
    [SerializeField] private GameObject powerUpPrefab;

    public Transform target { get; private set; }
    private GameObject powerUpInstance;
    private PowerUpPhysics physics;

    public void Init(Transform parent)
    {
        if (powerUpPrefab == null)
        {
            Debug.LogError("PowerUp prefab is not assigned!");
            return;
        }

        if (target == null)
        {
            powerUpInstance = Instantiate(powerUpPrefab, parent);
            target = powerUpInstance.transform;
            target.gameObject.SetActive(false);
        }

        physics = new PowerUpPhysics();
        AudioManager audioMgr = ServiceProvider.GetService<AudioManager>();
        physics.Initiate(target, powerUpSO, screenEdgesSO, this, audioMgr);
    }

    public void Activate(Vector3 position)
    {
        target.position = position;
        target.gameObject.SetActive(true);
    }

    public void Frame()
    {
        if (target.gameObject.activeSelf)
        {
            physics.Frame();
        }
    }

    public void CollideWithPaddle()
    {
        ActivatePowerUp();
        DestroyPowerUp();
    }

    private void ActivatePowerUp()
    {
        switch (powerUpSO.powerUpType)
        {
            case PowerUpType.Multiball:
                ActivateMultiball();
                break;
            case PowerUpType.WidePaddle:
                ActivateWidePaddle();
                break;
        }
    }

    private void ActivateMultiball()
    {
        int currentBalls = BallManager.GetActiveBalls().Count;
        int maxBalls = BallManager.GetMaxBalls();

        if (currentBalls >= maxBalls)
        {
            Debug.Log($"Max balls reached ({maxBalls}).");
            return;
        }

        int ballsToAdd = Mathf.Min(2, maxBalls - currentBalls);
        BallManager.SpawnAndLaunchMultipleBalls(ballsToAdd);
    }

    private void ActivateWidePaddle()
    {
        PaddleController paddleController = ServiceProvider.GetService<PaddleController>();
        if (paddleController != null)
        {
            paddleController.ActivateWidePaddle(1.5f, 5f);
            Debug.Log("Wide Paddle power-up activated.");
        }
    }

    public void DestroyPowerUp()
    {
        target.gameObject.SetActive(false);
        PowerUpManager.Unregister(this);
        PowerUpPool.Instance.ReturnToPool(this);
    }
}