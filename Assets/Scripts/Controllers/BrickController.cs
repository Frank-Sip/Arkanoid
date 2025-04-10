using System.Collections;
using UnityEngine;

public class BrickController : MonoBehaviour
{
    [SerializeField] public BrickSO brickConfig;
    [SerializeField] private Transform visual;

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
        BrickManager.Unregister(this);
        BrickPool.Instance.ReturnToPool(this);
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