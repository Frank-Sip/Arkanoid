using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Collisions
{
    public static bool CheckCollisions(Vector3 ballPosition, float radius)
    {
        Rect ballRect = new Rect(ballPosition.x - radius, ballPosition.y - radius, radius * 2, radius * 2);

        return PaddlePhysics.bounds.Overlaps(ballRect);
    }
}