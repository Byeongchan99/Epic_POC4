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

    [Header("Visual Feedback")]
    public Color brokenColor = new Color(0.3f, 0.3f, 0.3f, 0.5f); // 끊어진 선분 색상
    public Color weakpointColor = new Color(1f, 0.3f, 0.3f, 1f); // 약점 선분 색상 (빨간색)

    [Header("Weakpoint Settings")]
    [Range(0.1f, 0.5f)]
    public float weakpointRatio = 0.3f; // 약점 비율 (전체 선분의 30%)

    [Header("Segment Settings")]
    [Tooltip("한 획의 최대 길이. 이보다 길면 자동으로 분할됩니다.")]
    public float maxSegmentLength = 0.3f; // 한 선분의 최대 길이

    private List<LineRenderer> lineRenderers = new List<LineRenderer>();
    private List<LineSegment> segments = new List<LineSegment>();
    private float drawProgress = 0f;
    private bool isDrawing = false;
    private List<Vector2> patternPoints = new List<Vector2>();

    public enum CirclePattern
    {
        Circle,          // 원형
        Triangle,        // 삼각형
        Square,          // 사각형
        Pentagram,       // 오각별
        Hexagram,        // 육각별 (다윗의 별)
        Heptagram,       // 칠각별
        Octagram,        // 팔각별
        Spiral,          // 나선형
        DoublePentagram, // 이중 오각별
        CrossPattern,    // 십자가 패턴
        InfinitySymbol,  // 무한대 기호
        ComplexRune      // 복잡한 룬
    }

    public List<LineSegment> Segments => segments;
    public float DrawProgress => drawProgress / drawDuration;
    public bool IsComplete => drawProgress >= drawDuration;

    void Awake()
    {
        GeneratePattern();
        CreateLineRenderers();
    }

    /// <summary>
    /// 각 선분마다 별도의 LineRenderer 생성
    /// </summary>
    void CreateLineRenderers()
    {
        // 기존 LineRenderer들 정리
        foreach (var lr in lineRenderers)
        {
            if (lr != null) Destroy(lr.gameObject);
        }
        lineRenderers.Clear();

        // 각 선분마다 자식 GameObject + LineRenderer 생성
        for (int i = 0; i < segments.Count; i++)
        {
            bool isWeakpoint = segments[i].isWeakpoint;
            GameObject segmentObj = new GameObject($"Segment_{i}" + (isWeakpoint ? "_Weakpoint" : ""));
            segmentObj.transform.parent = transform;
            segmentObj.transform.localPosition = Vector3.zero;

            LineRenderer lr = segmentObj.AddComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Sprites/Default"));

            // 약점 선분은 다른 색상으로 표시
            Color segmentColor = isWeakpoint ? weakpointColor : circleColor;
            lr.startColor = segmentColor;
            lr.endColor = segmentColor;

            // 약점은 약간 더 두껍게 표시
            float segmentWidth = isWeakpoint ? lineWidth * 1.5f : lineWidth;
            lr.startWidth = segmentWidth;
            lr.endWidth = segmentWidth;

            lr.useWorldSpace = true;
            lr.sortingOrder = isWeakpoint ? 2 : 1; // 약점을 위에 그리기
            lr.positionCount = 2;

            // 선분의 시작점과 끝점 설정
            lr.SetPosition(0, segments[i].start);
            lr.SetPosition(1, segments[i].end);

            // 처음에는 비활성화 (그리기 애니메이션을 위해)
            lr.enabled = false;

            lineRenderers.Add(lr);
        }
    }

    void GeneratePattern()
    {
        patternPoints.Clear();

        switch (pattern)
        {
            case CirclePattern.Circle:
                GenerateCircle(1.5f, 32);
                break;
            case CirclePattern.Triangle:
                GeneratePolygon(1.5f, 3);
                break;
            case CirclePattern.Square:
                GeneratePolygon(1.5f, 4);
                break;
            case CirclePattern.Pentagram:
                GeneratePentagram(1.5f);
                break;
            case CirclePattern.Hexagram:
                GenerateHexagram(1.5f);
                break;
            case CirclePattern.Heptagram:
                GenerateStar(1.5f, 7);
                break;
            case CirclePattern.Octagram:
                GenerateStar(1.5f, 8);
                break;
            case CirclePattern.Spiral:
                GenerateSpiral(1.5f, 3);
                break;
            case CirclePattern.DoublePentagram:
                GenerateDoublePentagram(1.5f);
                break;
            case CirclePattern.CrossPattern:
                GenerateCross(1.5f);
                break;
            case CirclePattern.InfinitySymbol:
                GenerateInfinity(1.2f);
                break;
            case CirclePattern.ComplexRune:
                GenerateComplexRune(1.5f);
                break;
        }

        // 선분 생성 (긴 선분은 짧게 분할)
        List<LineSegment> tempSegments = new List<LineSegment>();
        for (int i = 0; i < patternPoints.Count - 1; i++)
        {
            Vector2 start = patternPoints[i] + (Vector2)transform.position;
            Vector2 end = patternPoints[i + 1] + (Vector2)transform.position;

            // 긴 선분을 짧게 분할
            List<LineSegment> subdivided = SubdivideSegment(start, end, maxSegmentLength);
            tempSegments.AddRange(subdivided);
        }

        // 약점 지정 (분할된 선분 기준)
        int totalSegments = tempSegments.Count;
        int weakpointCount = Mathf.Max(1, Mathf.RoundToInt(totalSegments * weakpointRatio));

        // 약점 인덱스 선택 (균등하게 분산)
        List<int> weakpointIndices = new List<int>();
        for (int i = 0; i < weakpointCount; i++)
        {
            int index = Mathf.FloorToInt((float)i * totalSegments / weakpointCount);
            weakpointIndices.Add(index);
        }

        // 약점 플래그 설정
        for (int i = 0; i < tempSegments.Count; i++)
        {
            tempSegments[i].isWeakpoint = weakpointIndices.Contains(i);
            segments.Add(tempSegments[i]);
        }
    }

    /// <summary>
    /// 긴 선분을 짧은 세그먼트로 분할
    /// </summary>
    List<LineSegment> SubdivideSegment(Vector2 start, Vector2 end, float maxLength)
    {
        List<LineSegment> result = new List<LineSegment>();
        float totalLength = Vector2.Distance(start, end);

        // 선분이 충분히 짧으면 그대로 반환
        if (totalLength <= maxLength)
        {
            result.Add(new LineSegment(start, end, false));
            return result;
        }

        // 필요한 분할 개수 계산
        int subdivisions = Mathf.CeilToInt(totalLength / maxLength);

        // 균등하게 분할
        for (int i = 0; i < subdivisions; i++)
        {
            float t1 = (float)i / subdivisions;
            float t2 = (float)(i + 1) / subdivisions;

            Vector2 segmentStart = Vector2.Lerp(start, end, t1);
            Vector2 segmentEnd = Vector2.Lerp(start, end, t2);

            result.Add(new LineSegment(segmentStart, segmentEnd, false));
        }

        return result;
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

    /// <summary>
    /// 정다각형 생성 (삼각형, 사각형 등)
    /// </summary>
    void GeneratePolygon(float radius, int sides)
    {
        for (int i = 0; i <= sides; i++)
        {
            float angle = i * Mathf.PI * 2f / sides - Mathf.PI / 2f;
            patternPoints.Add(new Vector2(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius
            ));
        }
    }

    /// <summary>
    /// N각 별 생성 (한붓그리기 별)
    /// </summary>
    void GenerateStar(float radius, int points)
    {
        // 한붓그리기 별 (2칸씩 건너뛰며 연결)
        for (int i = 0; i <= points; i++)
        {
            int index = (i * 2) % points;
            float angle = index * Mathf.PI * 2f / points - Mathf.PI / 2f;
            patternPoints.Add(new Vector2(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius
            ));
        }
    }

    /// <summary>
    /// 나선형 패턴
    /// </summary>
    void GenerateSpiral(float maxRadius, float turns)
    {
        int pointCount = 64;
        for (int i = 0; i <= pointCount; i++)
        {
            float t = i / (float)pointCount;
            float angle = t * Mathf.PI * 2f * turns;
            float radius = maxRadius * t;
            patternPoints.Add(new Vector2(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius
            ));
        }
    }

    /// <summary>
    /// 이중 오각별 (크고 작은 별)
    /// </summary>
    void GenerateDoublePentagram(float radius)
    {
        // 큰 별
        int[] order = { 0, 2, 4, 1, 3, 0 };
        foreach (int i in order)
        {
            float angle = i * Mathf.PI * 2f / 5f - Mathf.PI / 2f;
            patternPoints.Add(new Vector2(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius
            ));
        }

        // 작은 별 (회전)
        int startPoint = patternPoints.Count;
        foreach (int i in order)
        {
            float angle = i * Mathf.PI * 2f / 5f - Mathf.PI / 2f + Mathf.PI / 5f;
            patternPoints.Add(new Vector2(
                Mathf.Cos(angle) * radius * 0.6f,
                Mathf.Sin(angle) * radius * 0.6f
            ));
        }
        patternPoints.Add(patternPoints[startPoint]);
    }

    /// <summary>
    /// 십자가 패턴 (마법진 스타일)
    /// </summary>
    void GenerateCross(float size)
    {
        // 수평선
        patternPoints.Add(new Vector2(-size, 0));
        patternPoints.Add(new Vector2(size, 0));
        patternPoints.Add(new Vector2(0, 0));

        // 수직선
        patternPoints.Add(new Vector2(0, size));
        patternPoints.Add(new Vector2(0, -size));
        patternPoints.Add(new Vector2(0, 0));

        // 대각선 1
        patternPoints.Add(new Vector2(-size * 0.7f, size * 0.7f));
        patternPoints.Add(new Vector2(size * 0.7f, -size * 0.7f));
        patternPoints.Add(new Vector2(0, 0));

        // 대각선 2
        patternPoints.Add(new Vector2(size * 0.7f, size * 0.7f));
        patternPoints.Add(new Vector2(-size * 0.7f, -size * 0.7f));

        // 외곽 원
        int circleStart = patternPoints.Count;
        for (int i = 0; i <= 32; i++)
        {
            float angle = i * Mathf.PI * 2f / 32;
            patternPoints.Add(new Vector2(
                Mathf.Cos(angle) * size,
                Mathf.Sin(angle) * size
            ));
        }
    }

    /// <summary>
    /// 무한대 기호 (∞)
    /// </summary>
    void GenerateInfinity(float size)
    {
        int pointCount = 64;
        for (int i = 0; i <= pointCount; i++)
        {
            float t = i / (float)pointCount * Mathf.PI * 2f;
            float x = Mathf.Sin(t) * size;
            float y = Mathf.Sin(t) * Mathf.Cos(t) * size * 0.5f;
            patternPoints.Add(new Vector2(x, y));
        }
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
        int segmentsToShow = Mathf.FloorToInt(lineRenderers.Count * progress);

        // 완료된 선분들은 완전히 표시
        for (int i = 0; i < segmentsToShow && i < lineRenderers.Count; i++)
        {
            if (!lineRenderers[i].enabled)
            {
                lineRenderers[i].enabled = true;
            }
        }

        // 현재 그려지는 중인 선분 (부드러운 애니메이션)
        if (segmentsToShow < lineRenderers.Count)
        {
            float t = (lineRenderers.Count * progress) - segmentsToShow;
            LineRenderer currentLR = lineRenderers[segmentsToShow];

            if (!currentLR.enabled)
            {
                currentLR.enabled = true;
            }

            // 선분의 끝점을 lerp로 부드럽게 그리기
            Vector2 start = segments[segmentsToShow].start;
            Vector2 end = segments[segmentsToShow].end;
            Vector2 currentEnd = Vector2.Lerp(start, end, t);

            currentLR.SetPosition(0, start);
            currentLR.SetPosition(1, currentEnd);
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

            // 해당 선분의 LineRenderer 시각적 변경
            if (index < lineRenderers.Count && lineRenderers[index] != null)
            {
                LineRenderer lr = lineRenderers[index];

                // 색상을 어둡게 변경 (끊어진 표시)
                lr.startColor = brokenColor;
                lr.endColor = brokenColor;

                // 페이드 아웃 애니메이션
                StartCoroutine(FadeOutSegment(lr));
            }

            return true;
        }
        return false;
    }

    /// <summary>
    /// 끊어진 선분 페이드 아웃
    /// </summary>
    System.Collections.IEnumerator FadeOutSegment(LineRenderer lr)
    {
        float duration = 0.5f;
        float elapsed = 0f;
        Color startColor = brokenColor;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startColor.a, 0f, elapsed / duration);

            Color currentColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
            lr.startColor = currentColor;
            lr.endColor = currentColor;

            yield return null;
        }

        // 완전히 투명해지면 비활성화
        lr.enabled = false;
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
    /// 끊어진 약점 개수
    /// </summary>
    public int GetBrokenWeakpointCount()
    {
        int count = 0;
        foreach (var seg in segments)
        {
            if (seg.isWeakpoint && seg.isBroken) count++;
        }
        return count;
    }

    /// <summary>
    /// 전체 약점 개수
    /// </summary>
    public int GetTotalWeakpointCount()
    {
        int count = 0;
        foreach (var seg in segments)
        {
            if (seg.isWeakpoint) count++;
        }
        return count;
    }

    /// <summary>
    /// 일반 선분(보라색)이 잘렸는지 확인
    /// </summary>
    public bool HasBrokenNormalSegment()
    {
        foreach (var seg in segments)
        {
            // 약점이 아닌데 끊어진 선분이 있으면 true
            if (!seg.isWeakpoint && seg.isBroken)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 마법진 파괴 (페이드 아웃)
    /// </summary>
    public void Destroy()
    {
        Destroy(gameObject, 0.5f);
    }
}
