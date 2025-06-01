using UnityEngine;

[CreateAssetMenu(fileName = "BrickController", menuName = "GameObject/BrickControllerSO")]
public class BrickController : ScriptableObject
{
    [SerializeField] public BrickSO brickConfig;
    [SerializeField] private GameObject brickPrefab;
    [SerializeField] private float powerUpDropChance = 0.2f;

    public Rect bounds { get; private set; }
    [HideInInspector] public Transform target { get; private set; }
    public bool isEnabled { get; private set; }

    private Vector3 initialPosition;
    private bool wasDestroyed = false;

    public void Init(Transform parent)
    {
        if (brickPrefab == null)
        {
            Debug.LogError("Brick prefab is not assigned!");
            return;
        }

        if (target == null)
        {
            GameObject brickInstance = Instantiate(brickPrefab, parent);
            target = brickInstance.transform;
            target.gameObject.SetActive(false);
        }

        initialPosition = target.position;
        isEnabled = true;
        UpdateBounds();
    }

    public void Activate()
    {
        wasDestroyed = false;
        isEnabled = true;
        BrickPhysics.Initiate(target, target.GetChild(0), brickConfig, this);
        target.gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        isEnabled = false;
        target.gameObject.SetActive(false);
    }

    public void UpdateBounds()
    {
        BrickPhysics.UpdateBounds(target.transform, brickConfig, out Rect updatedBounds);
        bounds = updatedBounds;
    }

    public void SetBounds(Rect b)
    {
        bounds = b;
    }

    public void OnDestroyBrick()
    {
        if (wasDestroyed) return;

        wasDestroyed = true;
        isEnabled = false;

        if (Random.value < powerUpDropChance)
        {
            SpawnPowerUp();
        }

        BrickManager.Unregister(this);
        BrickPool.Instance.ReturnToPool(this);
    }

    public void Reset()
    {
        wasDestroyed = false;
        isEnabled = false;
        target.gameObject.SetActive(false);
        target.position = initialPosition;
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