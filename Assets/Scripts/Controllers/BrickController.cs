using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickController : MonoBehaviour
{
    public BrickSO brickConfig;
    public Rect bounds { get; private set; }

    public void Activate()
    {
        BrickPhysics.Initiate(transform, brickConfig, this);
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
        BrickManager.Unregister(this);
        BrickPool.Instance.ReturnToPool(this);
    }
}
