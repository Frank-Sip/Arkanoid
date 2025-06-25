using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BrickPhysics
{
    private static Dictionary<int, int> lastDamageFrameByBrick = new Dictionary<int, int>();
    
    public static void Initiate(Transform brickTransform, Transform visual, BrickSO config, BrickController controller)
    {
        Mesh mesh = visual.GetComponent<MeshFilter>()?.sharedMesh;
        if (mesh != null)
        {
            Bounds meshBounds = mesh.bounds;

            float baseWidth = meshBounds.size.x;
            float baseHeight = meshBounds.size.y;
            float baseLength = meshBounds.size.z;

            float scaleX = config.width / baseWidth;
            float scaleY = config.height / baseHeight;
            float scaleZ = config.length / baseLength;

            visual.localScale = new Vector3(scaleX, scaleY, scaleZ);

            Vector3 centerOffset = Vector3.Scale(meshBounds.center, visual.localScale);
            visual.localPosition = -centerOffset;

            visual.localRotation = Quaternion.identity;
        }
        else
        {
            visual.localScale = new Vector3(config.width, config.height, config.length);
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

        BrickController closestBrick = null;
        float minDistance = float.MaxValue;
        Vector2 closestPoint = Vector2.zero;
        Vector2 closestNormal = Vector2.zero;

        foreach (var brick in BrickManager.GetActiveBricks())
        {
            if (brick == null || !brick.target.gameObject.activeInHierarchy || !brick.isEnabled)
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
            float distance = contactVector.magnitude;

            if (distance < minDistance)
            {
                minDistance = distance;
                closestBrick = brick;
                closestPoint = closest;
                closestNormal = contactVector.normalized;
            }
        }

        if (closestBrick != null)
        {
            int brickId = closestBrick.GetInstanceID();
            
            if (lastDamageFrameByBrick.TryGetValue(brickId, out int lastFrame) && 
                lastFrame == Time.frameCount)
            {
                Vector3 correctionVector = closestNormal * (radius - minDistance);
                correction = new Vector3(correctionVector.x, correctionVector.y, 0f) * 1.5f;
                
                Vector3 direction3D = new Vector3(direction.x, direction.y, 0f);
                Vector3 normal3D = new Vector3(closestNormal.x, closestNormal.y, 0f);
                direction3D = Vector3.Reflect(direction3D, normal3D);
                direction = new Vector3(direction3D.x, direction3D.y, 0f);
                
                return true;
            }
            
            lastDamageFrameByBrick[brickId] = Time.frameCount;
            
            Vector3 directionVector = new Vector3(direction.x, direction.y, 0f);
            Vector3 normalVector = new Vector3(closestNormal.x, closestNormal.y, 0f);
            directionVector = Vector3.Reflect(directionVector, normalVector);
            direction = new Vector3(directionVector.x, directionVector.y, 0f);
            
            Vector3 correctionVec = closestNormal * (radius - minDistance);
            correction = new Vector3(correctionVec.x, correctionVec.y, 0f) * 1.5f;

            closestBrick.TakeDamage();
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