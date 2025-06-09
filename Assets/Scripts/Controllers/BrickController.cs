using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BrickController", menuName = "GameObject/BrickControllerSO")]
public class BrickController : ScriptableObject
{
    [SerializeField] public BrickSO brickConfig;
    [SerializeField] public GameObject brickPrefab;
    [SerializeField] private float powerUpDropChance = 0.2f;
    [SerializeField] private List<BrickTypeConfig> brickTypes;

    [HideInInspector] public Transform target;
    [HideInInspector] public BrickType currentType;
    
    public Rect bounds { get; private set; }
    public bool isEnabled { get; private set; } = true;
    private bool isSubscribed = false;

    public void Init(Transform parent = null)
    {
        if (target == null)
        {
            if (!isSubscribed)
            {
                EventManager.OnReset += Reset;
                isSubscribed = true;
            }
        }
    }

    public void Activate()
    {
        if (target != null)
        {
            target.gameObject.SetActive(true);
            isEnabled = true;
            InitializePhysics();
            UpdateBounds();
        }
    }

    public void InitializePhysics()
    {
        AudioManager audioMgr = ServiceProvider.GetService<AudioManager>();
        BrickPhysics.Initiate(target, target.GetChild(0), brickConfig, this);
    }

    public void SetBounds(Rect newBounds)
    {
        bounds = newBounds;
    }

    public void UpdateBounds()
    {
        BrickPhysics.UpdateBounds(target.transform, brickConfig, out Rect updatedBounds);
        SetBounds(updatedBounds);
    }

    public void TakeDamage()
    {
        BrickType nextType = GetNextLowerType(currentType);
        
        if (nextType == BrickType.None)
        {
            OnDestroyBrick();
            return;
        }

        SetBrickType(nextType);
    }

    public BrickController Clone()
    {
        var clone = Instantiate(this);
        clone.target = null;
        clone.isEnabled = true;
        clone.isSubscribed = false;
        return clone;
    }
    
    private BrickType GetNextLowerType(BrickType currentType)
    {
        switch (currentType)
        {
            case BrickType.Tough:
                return BrickType.Strong;
            case BrickType.Strong:
                return BrickType.Normal;
            case BrickType.Normal:
                return BrickType.Weak;
            case BrickType.Weak:
            default:
                return BrickType.None;
        }
    }
    
    public void SetBrickType(BrickType type)
    {
        currentType = type;
        var typeConfig = brickTypes.Find(x => x.type == type);
        if (typeConfig != null)
        {
            if (target != null)
            {
                Transform visual = target.GetChild(0);
                if (visual != null)
                {
                    typeConfig.atlas.ApplyAtlas(visual.gameObject);
                }
            }
        }
    }

    public void OnDestroyBrick()
    {
        isEnabled = false;
        if (Random.value < powerUpDropChance)
        {
            SpawnPowerUp();
        }

        BrickManager.Unregister(this);
        ServiceProvider.GetService<BrickPool>().ReturnToPool(this);
        ServiceProvider.GetService<UIManager>().SetCounterValue("BrickCounter", BrickManager.GetActiveBricks().Count);
    }

    public void Reset()
    {
        if (target != null)
        {
            target.gameObject.SetActive(false);
            target.localScale = Vector3.one;
            isEnabled = false;
            currentType = BrickType.None;
        }
    }

    private void OnDestroy()
    {
        if (isSubscribed)
        {
            EventManager.OnReset -= Reset;
        }
    }

    private void SpawnPowerUp()
    {
        if (!PowerUpManager.CanSpawnPowerUp()) return;

        PowerUpController powerUp = PowerUpManager.SpawnPowerUp(target.position);
        if (powerUp != null)
        {
            AssignRandomPowerUpType(powerUp);
        }
    }

    private void AssignRandomPowerUpType(PowerUpController powerUp)
    {
        PowerUpSO powerUpSO = powerUp.powerUpSO;
        if (powerUpSO == null) return;

        int randomType = Random.Range(0, 2);
        powerUpSO.powerUpType = randomType == 0 ? PowerUpType.Multiball : PowerUpType.WidePaddle;
        powerUp.ApplyAtlasBasedOnType();
    }
}