using UnityEngine;

[CreateAssetMenu(fileName = "PowerUpController", menuName = "GameObject/PowerUpControllerSO")]
public class PowerUpController : ScriptableObject
{
    [SerializeField] public PowerUpSO powerUpSO;
    [SerializeField] public GameObject powerUpPrefab;
    [SerializeField] private ScreenEdgesSO screenEdgesSO;
    [SerializeField] private AtlasApplier multiballAtlas;
    [SerializeField] private AtlasApplier widePaddleAtlas;

    [HideInInspector] public Transform target;
    private PowerUpPhysics physics;
    private bool isEnabled = true;

    public PowerUpController Clone()
    {
        var clone = Instantiate(this);
        clone.target = null;
        clone.isEnabled = true;
        return clone;
    }

    public void Init(Transform parent = null)
    {
        if (target == null)
        {
            GameObject powerUpInstance = Instantiate(powerUpPrefab);
            if (parent != null)
            {
                powerUpInstance.transform.SetParent(parent);
            }
            target = powerUpInstance.transform;
            target.gameObject.SetActive(false);
        }

        physics = new PowerUpPhysics();
        AudioManager audioMgr = ServiceProvider.GetService<AudioManager>();
        physics.Initiate(target, powerUpSO, screenEdgesSO, this, audioMgr);
        
        ApplyAtlasBasedOnType();
    }

    public void Activate()
    {
        if (target != null)
        {
            target.gameObject.SetActive(true);
            ApplyAtlasBasedOnType();
            isEnabled = true;
        }
    }

    public void Frame()
    {
        if (target != null && target.gameObject.activeSelf)
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
        }
    }
    
    public void ApplyAtlasBasedOnType()
    {
        if (target == null) return;
        
        Transform visual = target.GetChild(0);
        if (visual == null) return;

        AtlasApplier atlasToUse = powerUpSO.powerUpType switch
        {
            PowerUpType.Multiball => multiballAtlas,
            PowerUpType.WidePaddle => widePaddleAtlas,
            _ => multiballAtlas
        };

        if (atlasToUse != null)
        {
            atlasToUse.ApplyAtlas(visual.gameObject);
        }
    }

    public void DestroyPowerUp()
    {
        target.gameObject.SetActive(false);
        PowerUpManager.Unregister(this);
        ServiceProvider.GetService<PowerUpPool>().ReturnToPool(this);
    }

    public void Reset()
    {
        if (target != null)
        {
            target.gameObject.SetActive(false);
            target.localScale = Vector3.one;
            isEnabled = true;
        }
    }
}