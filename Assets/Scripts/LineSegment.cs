using UnityEngine;

/// <summary>
/// 마법진을 구성하는 선분 데이터
/// </summary>
public class LineSegment
{
    public Vector2 start;
    public Vector2 end;
    public bool isBroken = false;

    public LineSegment(Vector2 start, Vector2 end)
    {
        this.start = start;
        this.end = end;
    }

    /// <summary>
    /// 두 선분이 교차하는지 판정 (2D)
    /// </summary>
    public static bool Intersects(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        float d = (p2.x - p1.x) * (p4.y - p3.y) - (p2.y - p1.y) * (p4.x - p3.x);

        if (Mathf.Abs(d) < 0.0001f) return false; // 평행

        float t = ((p3.x - p1.x) * (p4.y - p3.y) - (p3.y - p1.y) * (p4.x - p3.x)) / d;
        float u = ((p3.x - p1.x) * (p2.y - p1.y) - (p3.y - p1.y) * (p2.x - p1.x)) / d;

        return t >= 0 && t <= 1 && u >= 0 && u <= 1;
    }

    /// <summary>
    /// 교차점 계산
    /// </summary>
    public static Vector2 GetIntersectionPoint(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        float d = (p2.x - p1.x) * (p4.y - p3.y) - (p2.y - p1.y) * (p4.x - p3.x);
        float t = ((p3.x - p1.x) * (p4.y - p3.y) - (p3.y - p1.y) * (p4.x - p3.x)) / d;

        return p1 + t * (p2 - p1);
    }
}
