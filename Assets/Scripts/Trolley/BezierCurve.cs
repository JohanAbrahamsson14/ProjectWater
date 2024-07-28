using UnityEngine;

public static class BezierCurve
{
    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = uu * p0; // (1-t)^2 * p0
        p += 2 * u * t * p1; // 2 * (1-t) * t * p1
        p += tt * p2; // t^2 * p2

        return p;
    }

    public static float GetApproximateLength(Vector3 p0, Vector3 p1, Vector3 p2, int subdivisions = 10)
    {
        float length = 0f;
        Vector3 previousPoint = p0;
        for (int i = 1; i <= subdivisions; i++)
        {
            float t = i / (float)subdivisions;
            Vector3 point = GetPoint(p0, p1, p2, t);
            length += Vector3.Distance(previousPoint, point);
            previousPoint = point;
        }
        return length;
    }
}