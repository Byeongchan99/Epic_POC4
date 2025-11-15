using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 마법진 생성 및 관리
/// </summary>
public class MagicCircle : MonoBehaviour
{
    [Header("Settings")]
    public float drawDuration = 3f;
    public Color circleColor = new Color(0.5f, 0.2f, 1f, 1f);
    public float lineWidth = 0.1f;

    [Header("Pattern")]
    public CirclePattern pattern = CirclePattern.Pentagram;

    private LineRenderer lineRenderer;
    private List<LineSegment> segments = new List<LineSegment>();
    private float drawProgress = 0f;
    private bool isDrawing = false;
    private List<Vector2> patternPoints = new List<Vector2>();

    public enum CirclePattern
    {
        Circle,
        Pentagram,
        Hexagram,
        ComplexRune
    }

    public List<LineSegment> Segments => segments;
    public float DrawProgress => drawProgress / drawDuration;
    public bool IsComplete => drawProgress >= drawDuration;

    void Awake()
    {
        SetupLineRenderer();
        GeneratePattern();
    }

    void SetupLineRenderer()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = circleColor;
        lineRenderer.endColor = circleColor;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.useWorldSpace = true;
        lineRenderer.sortingOrder = 1;
    }

    void GeneratePattern()
    {
        patternPoints.Clear();

        switch (pattern)
        {
            case CirclePattern.Circle:
                GenerateCircle(1f, 32);
                break;
            case CirclePattern.Pentagram:
                GeneratePentagram(1.5f);
                break;
            case CirclePattern.Hexagram:
                GenerateHexagram(1.5f);
                break;
            case CirclePattern.ComplexRune:
                GenerateComplexRune(1.5f);
                break;
        }

        // 선분 생성
        for (int i = 0; i < patternPoints.Count - 1; i++)
        {
            segments.Add(new LineSegment(
                patternPoints[i] + (Vector2)transform.position,
                patternPoints[i + 1] + (Vector2)transform.position
            ));
        }
    }

    void GenerateCircle(float radius, int points)
    {
        for (int i = 0; i <= points; i++)
        {
            float angle = i * Mathf.PI * 2f / points;
            patternPoints.Add(new Vector2(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius
            ));
        }
    }

    void GeneratePentagram(float radius)
    {
        // 별 5각형 (한붓그리기)
        int[] order = { 0, 2, 4, 1, 3, 0 }; // 별 그리기 순서
        foreach (int i in order)
        {
            float angle = i * Mathf.PI * 2f / 5f - Mathf.PI / 2f;
            patternPoints.Add(new Vector2(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius
            ));
        }
    }

    void GenerateHexagram(float radius)
    {
        // 육각별 (다윗의 별)
        for (int i = 0; i <= 6; i++)
        {
            float angle = i * Mathf.PI * 2f / 6f - Mathf.PI / 2f;
            patternPoints.Add(new Vector2(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius
            ));
        }

        // 반대편 삼각형
        for (int i = 0; i <= 6; i++)
        {
            float angle = i * Mathf.PI * 2f / 6f - Mathf.PI / 2f + Mathf.PI / 3f;
            patternPoints.Add(new Vector2(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius
            ));
        }
    }

    void GenerateComplexRune(float radius)
    {
        // 복잡한 룬 마법진
        GenerateCircle(radius, 24);

        // 내부 별
        int pointCount = patternPoints.Count;
        for (int i = 0; i < 8; i++)
        {
            float angle = i * Mathf.PI * 2f / 8f;
            patternPoints.Add(new Vector2(
                Mathf.Cos(angle) * radius * 0.5f,
                Mathf.Sin(angle) * radius * 0.5f
            ));
        }
        patternPoints.Add(patternPoints[pointCount]);
    }

    public void StartDrawing()
    {
        isDrawing = true;
        drawProgress = 0f;
    }

    void Update()
    {
        if (isDrawing && !IsComplete)
        {
            drawProgress += Time.deltaTime;
            UpdateVisual();
        }
    }

    void UpdateVisual()
    {
        float progress = Mathf.Clamp01(DrawProgress);
        int pointsToShow = Mathf.FloorToInt(patternPoints.Count * progress);

        lineRenderer.positionCount = pointsToShow;
        for (int i = 0; i < pointsToShow; i++)
        {
            lineRenderer.SetPosition(i, patternPoints[i] + (Vector2)transform.position);
        }

        // 마지막 점은 부드럽게 그려지기
        if (pointsToShow > 0 && pointsToShow < patternPoints.Count)
        {
            float t = (patternPoints.Count * progress) - pointsToShow;
            Vector2 currentPoint = Vector2.Lerp(
                patternPoints[pointsToShow - 1],
                patternPoints[pointsToShow],
                t
            );
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(pointsToShow, currentPoint + (Vector2)transform.position);
        }
    }

    /// <summary>
    /// 선분을 끊음
    /// </summary>
    public bool BreakSegment(int index)
    {
        if (index >= 0 && index < segments.Count && !segments[index].isBroken)
        {
            segments[index].isBroken = true;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 끊어진 선분 개수
    /// </summary>
    public int GetBrokenCount()
    {
        int count = 0;
        foreach (var seg in segments)
        {
            if (seg.isBroken) count++;
        }
        return count;
    }

    /// <summary>
    /// 마법진 파괴 (페이드 아웃)
    /// </summary>
    public void Destroy()
    {
        Destroy(gameObject, 0.5f);
    }
}
