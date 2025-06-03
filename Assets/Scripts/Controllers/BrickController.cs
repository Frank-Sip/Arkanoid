using UnityEngine;

[CreateAssetMenu(fileName = "BrickController", menuName = "GameObject/BrickControllerSO")]
public class BrickController : ScriptableObject
{
    [SerializeField] public BrickSO brickConfig;
    [SerializeField] public GameObject brickPrefab;
    [SerializeField] private float powerUpDropChance = 0.2f;

    [HideInInspector] public Transform target;
    public Rect bounds { get; private set; }
    public bool isEnabled { get; private set; } = true;
    private bool isSubscribed = false;

    public BrickController Clone()
    {
        var clone = Instantiate(this);
        clone.target = null;
        clone.isEnabled = true;
        clone.isSubscribed = false;
        return clone;
    }

    public void Init(Transform parent = null)
    {
        if (target == null)
        {
            GameObject brickInstance = Instantiate(brickPrefab);
            if (parent != null)
            {
                brickInstance.transform.SetParent(parent);
            }
            target = brickInstance.transform;
            target.gameObject.SetActive(false);

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

    public void OnDestroyBrick()
    {
        isEnabled = false;
        if (Random.value < powerUpDropChance)
        {
            //SpawnPowerUp();
        }

        BrickManager.Unregister(this);
        BrickPool.Instance.ReturnToPool(this);
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

        Transform modelTransform = powerUp.target.transform.GetChild(0);
        AtlasApplier atlasApplier = modelTransform.GetComponent<AtlasApplier>() ?? modelTransform.GetComponentInChildren<AtlasApplier>();
        if (atlasApplier == null) return;

        int randomType = Random.Range(0, 2);
        powerUpSO.powerUpType = randomType == 0 ? PowerUpType.Multiball : PowerUpType.WidePaddle;
        atlasApplier.atlasType = randomType == 0 ? AtlasType.Blue : AtlasType.Green;
        atlasApplier.ApplyAtlas();
    }
}