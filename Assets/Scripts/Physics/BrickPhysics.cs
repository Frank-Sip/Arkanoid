using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BrickPhysics
{
    public static void Initiate(Transform brickTransform, Transform visual, BrickSO config, BrickController controller)
    {
        Mesh mesh = visual.GetComponent<MeshFilter>()?.sharedMesh;
        if (mesh != null)
        {
            Bounds meshBounds = mesh.bounds;

            float baseWidth = meshBounds.size.x;
            float baseHeight = meshBounds.size.y;

            float scaleX = config.width / baseWidth;
            float scaleY = config.height / baseHeight;

            visual.localScale = new Vector3(scaleX, scaleY, 1f);

            Vector3 centerOffset = Vector3.Scale(meshBounds.center, visual.localScale);
            visual.localPosition = -centerOffset;

            visual.localRotation = Quaternion.identity;
        }
        else
        {
            visual.localScale = new Vector3(config.width, config.height, 1f);
            visual.localPosition = Vector3.zero;
        }

        Vector3 pos = brickTransform.position;
        Rect bounds = new Rect(pos.x - config.width / 2f, pos.y - config.height / 2f, config.width, config.height);
        controller.SetBounds(bounds);
    }


    public static void UpdateBounds(Transform t, BrickSO config, out Rect bounds)
    {
        Vector3 pos = t.position;
        bounds = new Rect(pos.x - config.width / 2f, pos.y - config.height / 2f, config.width, config.height);
    }

    public static bool CheckCollision(Vector3 ballPos, float radius, ref Vector3 direction, out Vector3 correction)
    {
        Rect ballRect = new Rect(ballPos.x - radius, ballPos.y - radius, radius * 2, radius * 2);
        correction = Vector3.zero;

        foreach (var brick in BrickManager.GetActiveBricks())
        {
            if (brick == null || !brick.gameObject.activeInHierarchy || !brick.enabled)
                continue;

            if (!brick.bounds.Overlaps(ballRect)) continue;

            Vector2 ballCenter = ballPos;
            Vector2 brickCenter = brick.bounds.center;
            Vector2 delta = ballCenter - brickCenter;

            float bx = brick.bounds.width / 2f;
            float by = brick.bounds.height / 2f;

            float dx = Mathf.Clamp(delta.x, -bx, bx);
            float dy = Mathf.Clamp(delta.y, -by, by);
            Vector2 closest = brickCenter + new Vector2(dx, dy);

            Vector2 contactVector = ballCenter - closest;
            Vector2 normal = contactVector.normalized;

            direction = Vector3.Reflect(direction, normal);
            correction = normal * (radius - contactVector.magnitude);

            brick.OnDestroyBrick();
            return true;
        }

        return false;
    }

#if UNITY_EDITOR
    public static void DrawVisualGizmo(Transform visual)
    {
        Mesh mesh = visual.GetComponent<MeshFilter>()?.sharedMesh;
        if (mesh == null) return;

        Gizmos.color = Color.cyan;
        
        Matrix4x4 localToWorld = visual.localToWorldMatrix;
        Gizmos.matrix = localToWorld;
        Gizmos.DrawWireCube(mesh.bounds.center, mesh.bounds.size);
        Gizmos.matrix = Matrix4x4.identity;
    }
#endif
}
