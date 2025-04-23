using System.Collections;
using UnityEngine;

public class BrickController : MonoBehaviour
{
    [SerializeField] public BrickSO brickConfig;
    [SerializeField] private Transform visual;
    [SerializeField] private float powerUpDropChance = 0.2f; 

    public Rect bounds { get; private set; }

    private Vector3 initialPosition;
    private bool isSubscribed = false;

    public void Activate()
    {
        BrickPhysics.Initiate(transform, visual, brickConfig, this);
        gameObject.SetActive(true);

        initialPosition = transform.position;

        if (!isSubscribed)
        {
            EventManager.OnReset += ResetBrick;
            isSubscribed = true;
        }
    }

    public void UpdateBounds()
    {
        BrickPhysics.UpdateBounds(transform, brickConfig, out Rect updatedBounds);
        bounds = updatedBounds;
    }

    public void SetBounds(Rect b)
    {
        bounds = b;
    }

    public void OnDestroyBrick()
    {
        if (Random.value < powerUpDropChance)
        {
            SpawnPowerUp();
        }
        
        BrickManager.Unregister(this);
        BrickPool.Instance.ReturnToPool(this);
    }
    
    private void SpawnPowerUp()
    {
        if (!PowerUpManager.CanSpawnPowerUp())
        {
            return;
        }
        
        PowerUpController powerUp = PowerUpManager.SpawnPowerUp(transform.position);
        
        if (powerUp != null)
        {
            AssignRandomPowerUpType(powerUp);
        }
    }

    private void AssignRandomPowerUpType(PowerUpController powerUp)
    {

        PowerUpSO powerUpSO = powerUp.GetComponent<PowerUpController>().powerUpSO;
        if (powerUpSO == null)
        {
            Debug.LogError("No se pudo encontrar el PowerUpSO en el power-up");
            return;
        }
        
        int randomType = Random.Range(0, 2); 
        

        if (randomType == 0)
        {
            powerUpSO.powerUpType = PowerUpType.Multiball;
            Debug.Log("Generado power-up: Multiball");
        }
        else
        {
            powerUpSO.powerUpType = PowerUpType.WidePaddle;
            Debug.Log("Generado power-up: WidePaddle");
        }
    }

    private void ResetBrick()
    {
        transform.position = initialPosition;
        Activate();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(bounds.center, bounds.size);

        if (visual != null)
        {
            Gizmos.color = Color.red;
            Bounds meshBounds = visual.GetComponent<MeshFilter>()?.sharedMesh?.bounds ?? new Bounds(Vector3.zero, Vector3.one);
            Vector3 size = Vector3.Scale(meshBounds.size, visual.lossyScale);
            Vector3 center = visual.position + Vector3.Scale(meshBounds.center, visual.lossyScale);
            Gizmos.DrawWireCube(center, size);
        }
    }
#endif
}