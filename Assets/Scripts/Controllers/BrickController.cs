using System.Collections;
using UnityEngine;

public class BrickController : MonoBehaviour
{
    [SerializeField] public BrickSO brickConfig;
    [SerializeField] private Transform visual;
    [SerializeField] private float powerUpDropChance = 0.1f; // 10% de probabilidad

    public Rect bounds { get; private set; }

    public void Activate()
    {
        BrickPhysics.Initiate(transform, visual, brickConfig, this);
        gameObject.SetActive(true);
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
        // Intentar soltar un power-up
        if (Random.value < powerUpDropChance)
        {
            PowerUpManager.SpawnPowerUp(transform.position);
        }

        BrickManager.Unregister(this);
        BrickPool.Instance.ReturnToPool(this);
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