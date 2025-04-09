using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BrickPhysics
{
    public static void Initiate(Transform t, BrickSO config, BrickController controller)
    {
        t.localScale = new Vector3(config.width, config.height, 1f);
        
        Vector3 pos = t.position;
        Rect bounds = new Rect(pos.x - config.width / 2f, pos.y - config.height / 2f, config.width, config.height);
        controller.SetBounds(bounds);
        
        BrickManager.Register(controller);
    }

    public static void UpdateBounds(Transform t, BrickSO config, out Rect bounds)
    {
        Vector3 pos = t.position;
        bounds = new Rect(pos.x - config.width / 2f, pos.y - config.height / 2f, config.width, config.height);
    }

    public static bool CheckCollision(Vector3 ballPos, float radius)
    {
        Rect ballRect = new Rect(ballPos.x - radius, ballPos.y - radius, radius * 2, radius * 2);

        foreach (var brick in BrickManager.GetActiveBricks())
        {
            if (brick == null || !brick.gameObject.activeInHierarchy || !brick.enabled)
            {
                continue;
            }

            if (brick.bounds.Overlaps(ballRect))
            {
                brick.OnDestroyBrick();
                return true;
            }
        }

        return false;
    }
}