using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PowerUpController", menuName = "GameObject/PowerUpControllerSO")]
public class PowerUpController : ScriptableObject
{
    [SerializeField] public GameObject powerUpPrefab;
    [SerializeField] private ScreenEdgesSO screenEdgesSO;
    [SerializeField] public List<PowerUpSO> powerUpConfigs;

    [HideInInspector] public Transform target;
    [HideInInspector] public PowerUpSO currentPowerUp;
    private PowerUpPhysics physics;
    private bool isEnabled = true;
    private int extraLife = 1;

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
        physics.Initiate(target, currentPowerUp, screenEdgesSO, this, audioMgr);
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
        if (currentPowerUp == null) return;

        switch (currentPowerUp.powerUpType)
        {
            case PowerUpType.Multiball:
                ActivateMultiball();
                break;
            case PowerUpType.WidePaddle:
                ActivateWidePaddle();
                break;
            case PowerUpType.ExtraLife:
                ActivateExtraLife();
                break;
            case PowerUpType.SpeedUp:
                ActivateSpeedUp();
                break;
        }
    }

    private void ActivateMultiball()
    {
        BallManager.SpawnAndLaunchMultipleBalls(2);
    }

    private void ActivateWidePaddle()
    {
        PaddleController paddleController = ServiceProvider.GetService<PaddleController>();
        if (paddleController != null)
        {
            paddleController.ActivateWidePaddle(1.5f, 5f);
        }
    }

    private void ActivateExtraLife()
    {
        PaddleController paddleController = ServiceProvider.GetService<PaddleController>();
        if (paddleController != null)
        {
            paddleController.AddLife(extraLife);
        }
    }
    
    private void ActivateSpeedUp()
    {
        PaddleController paddleController = ServiceProvider.GetService<PaddleController>();
        if (paddleController != null)
        {
            paddleController.ActivateSpeedBoost(1.5f, 5f);
        }
    }

    public void ApplyAtlasBasedOnType()
    {
        if (target == null || currentPowerUp == null) return;

        Transform visual = target.GetChild(0);
        if (visual == null) return;

        if (currentPowerUp.atlas != null)
        {
            currentPowerUp.atlas.ApplyAtlas(visual.gameObject);
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