using UnityEngine;

public static class GameMath
{
    public static Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1f - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;

        return p;
    }

    public static Vector3 CalculateBezierDir(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float diff = 0.01f;
        float startValue = Mathf.Clamp01(t - (diff / 2));
        float endValue = Mathf.Clamp01(t + (diff / 2));
        Vector3 point1 = CalculateBezierPoint(startValue, p0, p1, p2, p3);
        Vector3 point2 = CalculateBezierPoint(endValue, p0, p1, p2, p3);

        return (point2 - point1).normalized;
    }
}