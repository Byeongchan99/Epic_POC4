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
    [Tooltip("마법진의 크기 (반지름)")]
    public float radius = 1.5f;

    [Header("Stage Time Settings")]
    [Tooltip("이 스테이지의 제한 시간 (0이면 GameManager의 기본 시간 사용)")]
    public float stageTimeLimit = 0f;

    [Header("Visual Feedback")]
    public Color brokenColor = new Color(0.3f, 0.3f, 0.3f, 0.5f); // 끊어진 선분 색상
    public Color weakpointColor = new Color(1f, 0.3f, 0.3f, 1f); // 약점 선분 색상 (빨간색)

    [Header("Weakpoint Settings")]
    [Range(0.1f, 0.5f)]
    public float weakpointRatio = 0.3f; // 약점 비율 (전체 선분의 30%)

    [Tooltip("약점 분포 패턴을 선택합니다.")]
    public WeakpointDistribution weakpointDistribution = WeakpointDistribution.Uniform;

    [Tooltip("Custom 분포를 선택했을 때, 약점으로 지정할 선분 인덱스들 (0부터 시작)")]
    public int[] customWeakpointIndices;

    [Header("Segment Settings")]
    [Tooltip("한 획의 최대 길이. 이보다 길면 자동으로 분할됩니다.")]
    public float maxSegmentLength = 0.3f; // 한 선분의 최대 길이

    private List<LineRenderer> lineRenderers = new List<LineRenderer>();
    private List<LineSegment> segments = new List<LineSegment>();
    private float drawProgress = 0f;
    private bool isDrawing = false;
    private List<Vector2> patternPoints = new List<Vector2>();

    public enum WeakpointDistribution
    {
        Uniform,    // 균등 분산 (기본)
        Front,      // 앞쪽에 집중
        Back,       // 뒤쪽에 집중
        Random,     // 랜덤
        Custom      // 직접 인덱스 지정
    }

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
                GenerateCircle(radius, 32);
                break;
            case CirclePattern.Triangle:
                GeneratePolygon(radius, 3);
                break;
            case CirclePattern.Square:
                GeneratePolygon(radius, 4);
                break;
            case CirclePattern.Pentagram:
                GeneratePentagram(radius);
                break;
            case CirclePattern.Hexagram:
                GenerateStar(radius, 12, 5); // 12개 점으로 6각별 모양 (5칸씩 건너뛰기)
                break;
            case CirclePattern.Heptagram:
                GenerateStar(radius, 7, 2); // 7각별, 2칸씩 건너뛰기
                break;
            case CirclePattern.Octagram:
                GenerateStar(radius, 8, 3); // 8각별, 3칸씩 건너뛰기
                break;
            case CirclePattern.Spiral:
                GenerateSpiral(radius, 3);
                break;
            case CirclePattern.DoublePentagram:
                GenerateDoublePentagram(radius);
                break;
            case CirclePattern.CrossPattern:
                GenerateCross(radius);
                break;
            case CirclePattern.InfinitySymbol:
                GenerateInfinity(radius * 0.8f);  // 무한대는 약간 작게
                break;
            case CirclePattern.ComplexRune:
                GenerateComplexRune(radius);
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
        List<int> weakpointIndices = GetWeakpointIndices(totalSegments);

        // 약점 플래그 설정
        for (int i = 0; i < tempSegments.Count; i++)
        {
            tempSegments[i].isWeakpoint = weakpointIndices.Contains(i);
            segments.Add(tempSegments[i]);
        }
    }

    /// <summary>
    /// 약점 인덱스 선택 (분포 패턴에 따라)
    /// </summary>
    List<int> GetWeakpointIndices(int totalSegments)
    {
        List<int> indices = new List<int>();
        int weakpointCount = Mathf.Max(1, Mathf.RoundToInt(totalSegments * weakpointRatio));

        switch (weakpointDistribution)
        {
            case WeakpointDistribution.Uniform:
                // 균등 분산
                for (int i = 0; i < weakpointCount; i++)
                {
                    int index = Mathf.FloorToInt((float)i * totalSegments / weakpointCount);
                    indices.Add(index);
                }
                break;

            case WeakpointDistribution.Front:
                // 앞쪽에 집중 (처음 50% 구간에만 배치)
                int frontRange = Mathf.Max(1, totalSegments / 2);
                for (int i = 0; i < weakpointCount; i++)
                {
                    int index = Mathf.FloorToInt((float)i * frontRange / weakpointCount);
                    indices.Add(index);
                }
                break;

            case WeakpointDistribution.Back:
                // 뒤쪽에 집중 (마지막 50% 구간에만 배치)
                int backStart = totalSegments / 2;
                int backRange = totalSegments - backStart;
                for (int i = 0; i < weakpointCount; i++)
                {
                    int index = backStart + Mathf.FloorToInt((float)i * backRange / weakpointCount);
                    indices.Add(index);
                }
                break;

            case WeakpointDistribution.Random:
                // 랜덤 배치
                System.Random random = new System.Random();
                HashSet<int> randomIndices = new HashSet<int>();
                while (randomIndices.Count < weakpointCount)
                {
                    int randomIndex = random.Next(0, totalSegments);
                    randomIndices.Add(randomIndex);
                }
                indices.AddRange(randomIndices);
                break;

            case WeakpointDistribution.Custom:
                // 직접 지정한 인덱스 사용
                if (customWeakpointIndices != null && customWeakpointIndices.Length > 0)
                {
                    foreach (int customIndex in customWeakpointIndices)
                    {
                        // 유효한 인덱스만 추가
                        if (customIndex >= 0 && customIndex < totalSegments)
                        {
                            indices.Add(customIndex);
                        }
                    }

                    // Custom 인덱스가 하나도 없으면 Uniform으로 폴백
                    if (indices.Count == 0)
                    {
                        Debug.LogWarning("Custom weakpoint indices are invalid. Falling back to Uniform distribution.");
                        for (int i = 0; i < weakpointCount; i++)
                        {
                            int index = Mathf.FloorToInt((float)i * totalSegments / weakpointCount);
                            indices.Add(index);
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("Custom weakpoint distribution selected but no indices provided. Using Uniform.");
                    // Custom이지만 인덱스가 없으면 Uniform으로 폴백
                    for (int i = 0; i < weakpointCount; i++)
                    {
                        int index = Mathf.FloorToInt((float)i * totalSegments / weakpointCount);
                        indices.Add(index);
                    }
                }
                break;
        }

        return indices;
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
    void GenerateStar(float radius, int points, int skip = 2)
    {
        // 한붓그리기 별 (skip칸씩 건너뛰며 연결)
        for (int i = 0; i <= points; i++)
        {
            int index = (i * skip) % points;
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
    /// 선분이 완전히 그려졌는지 확인
    /// </summary>
    public bool IsSegmentFullyDrawn(int index)
    {
        if (index < 0 || index >= lineRenderers.Count)
            return false;

        // 그리기가 완료되지 않았으면 false
        if (!IsComplete && isDrawing)
        {
            float progress = Mathf.Clamp01(DrawProgress);
            int segmentsFullyDrawn = Mathf.FloorToInt(lineRenderers.Count * progress);

            // 현재 인덱스가 완전히 그려진 선분 범위에 있는지 확인
            return index < segmentsFullyDrawn;
        }

        // 그리기가 완료되었으면 모든 선분이 그려진 것으로 간주
        return true;
    }

    /// <summary>
    /// 선분을 끊음 (그려진 선분만 끊을 수 있음)
    /// </summary>
    public bool BreakSegment(int index)
    {
        if (index >= 0 && index < segments.Count && !segments[index].isBroken)
        {
            // 그려지지 않은 선분은 자를 수 없음
            if (!IsSegmentFullyDrawn(index))
            {
                return false;
            }

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

#if UNITY_EDITOR
    /// <summary>
    /// Editor에서 약점 위치 미리보기 (Scene 뷰에서만 표시)
    /// </summary>
    void OnDrawGizmosSelected()
    {
        // Play 모드가 아닐 때만 미리보기 표시
        if (UnityEngine.Application.isPlaying) return;

        // 임시로 패턴 생성
        List<Vector2> previewPoints = new List<Vector2>();
        GeneratePatternPreview(previewPoints);

        if (previewPoints.Count < 2) return;

        // 임시 선분 생성 (분할 포함)
        List<LineSegment> previewSegments = new List<LineSegment>();
        for (int i = 0; i < previewPoints.Count - 1; i++)
        {
            Vector2 start = previewPoints[i] + (Vector2)transform.position;
            Vector2 end = previewPoints[i + 1] + (Vector2)transform.position;

            List<LineSegment> subdivided = SubdivideSegment(start, end, maxSegmentLength);
            previewSegments.AddRange(subdivided);
        }

        // 약점 인덱스 계산
        List<int> weakpointIndices = GetWeakpointIndices(previewSegments.Count);

        // 선분 그리기
        for (int i = 0; i < previewSegments.Count; i++)
        {
            bool isWeakpoint = weakpointIndices.Contains(i);

            // 색상 설정
            if (isWeakpoint)
            {
                UnityEditor.Handles.color = weakpointColor;
            }
            else
            {
                UnityEditor.Handles.color = circleColor;
            }

            // 선 두께 설정
            float thickness = isWeakpoint ? lineWidth * 1.5f : lineWidth;
            UnityEditor.Handles.DrawAAPolyLine(thickness * 10f, previewSegments[i].start, previewSegments[i].end);

            // 약점 인덱스 표시
            if (isWeakpoint)
            {
                Vector2 midPoint = (previewSegments[i].start + previewSegments[i].end) / 2f;
                UnityEditor.Handles.Label(midPoint, $"#{i}", new UnityEngine.GUIStyle()
                {
                    normal = new UnityEngine.GUIStyleState() { textColor = Color.red },
                    fontSize = 12,
                    fontStyle = UnityEngine.FontStyle.Bold
                });
            }
        }

        // 전체 선분 개수 표시
        Vector3 labelPos = transform.position + Vector3.up * 3f;
        UnityEditor.Handles.Label(labelPos,
            $"Total Segments: {previewSegments.Count}\nWeakpoints: {weakpointIndices.Count}",
            new UnityEngine.GUIStyle()
            {
                normal = new UnityEngine.GUIStyleState() { textColor = Color.white },
                fontSize = 14,
                fontStyle = UnityEngine.FontStyle.Bold,
                alignment = UnityEngine.TextAnchor.MiddleCenter
            });
    }

    /// <summary>
    /// 미리보기용 패턴 생성 (GeneratePattern과 동일하지만 별도 리스트 사용)
    /// </summary>
    void GeneratePatternPreview(List<Vector2> points)
    {
        switch (pattern)
        {
            case CirclePattern.Circle:
                GenerateCirclePreview(points, radius, 32);
                break;
            case CirclePattern.Triangle:
                GeneratePolygonPreview(points, radius, 3);
                break;
            case CirclePattern.Square:
                GeneratePolygonPreview(points, radius, 4);
                break;
            case CirclePattern.Pentagram:
                GeneratePentagramPreview(points, radius);
                break;
            case CirclePattern.Hexagram:
                GenerateStarPreview(points, radius, 12, 5); // 12개 점으로 6각별 모양 (5칸씩 건너뛰기)
                break;
            case CirclePattern.Heptagram:
                GenerateStarPreview(points, radius, 7, 2); // 7각별, 2칸씩 건너뛰기
                break;
            case CirclePattern.Octagram:
                GenerateStarPreview(points, radius, 8, 3); // 8각별, 3칸씩 건너뛰기
                break;
            case CirclePattern.Spiral:
                GenerateSpiralPreview(points, radius, 3);
                break;
            case CirclePattern.DoublePentagram:
                GenerateDoublePentagramPreview(points, radius);
                break;
            case CirclePattern.CrossPattern:
                GenerateCrossPreview(points, radius);
                break;
            case CirclePattern.InfinitySymbol:
                GenerateInfinityPreview(points, radius * 0.8f);
                break;
            case CirclePattern.ComplexRune:
                GenerateComplexRunePreview(points, radius);
                break;
        }
    }

    void GenerateCirclePreview(List<Vector2> points, float radius, int segments)
    {
        for (int i = 0; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            points.Add(new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius));
        }
    }

    void GeneratePolygonPreview(List<Vector2> points, float radius, int sides)
    {
        for (int i = 0; i <= sides; i++)
        {
            float angle = i * Mathf.PI * 2f / sides - Mathf.PI / 2f;
            points.Add(new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius));
        }
    }

    void GeneratePentagramPreview(List<Vector2> points, float radius)
    {
        int[] order = { 0, 2, 4, 1, 3, 0 };
        foreach (int i in order)
        {
            float angle = i * Mathf.PI * 2f / 5f - Mathf.PI / 2f;
            points.Add(new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius));
        }
    }

    void GenerateHexagramPreview(List<Vector2> points, float radius)
    {
        for (int i = 0; i <= 6; i++)
        {
            float angle = i * Mathf.PI * 2f / 6f - Mathf.PI / 2f;
            points.Add(new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius));
        }
        for (int i = 0; i <= 6; i++)
        {
            float angle = i * Mathf.PI * 2f / 6f - Mathf.PI / 2f + Mathf.PI / 3f;
            points.Add(new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius));
        }
    }

    void GenerateStarPreview(List<Vector2> points, float radius, int starPoints, int skip = 2)
    {
        for (int i = 0; i <= starPoints; i++)
        {
            int index = (i * skip) % starPoints;
            float angle = index * Mathf.PI * 2f / starPoints - Mathf.PI / 2f;
            points.Add(new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius));
        }
    }

    void GenerateSpiralPreview(List<Vector2> points, float maxRadius, float turns)
    {
        int pointCount = 64;
        for (int i = 0; i <= pointCount; i++)
        {
            float t = i / (float)pointCount;
            float angle = t * Mathf.PI * 2f * turns;
            float r = maxRadius * t;
            points.Add(new Vector2(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r));
        }
    }

    void GenerateDoublePentagramPreview(List<Vector2> points, float radius)
    {
        int[] order = { 0, 2, 4, 1, 3, 0 };
        foreach (int i in order)
        {
            float angle = i * Mathf.PI * 2f / 5f - Mathf.PI / 2f;
            points.Add(new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius));
        }
        int startPoint = points.Count;
        foreach (int i in order)
        {
            float angle = i * Mathf.PI * 2f / 5f - Mathf.PI / 2f + Mathf.PI / 5f;
            points.Add(new Vector2(Mathf.Cos(angle) * radius * 0.6f, Mathf.Sin(angle) * radius * 0.6f));
        }
        points.Add(points[startPoint]);
    }

    void GenerateCrossPreview(List<Vector2> points, float size)
    {
        points.Add(new Vector2(-size, 0));
        points.Add(new Vector2(size, 0));
        points.Add(new Vector2(0, 0));
        points.Add(new Vector2(0, size));
        points.Add(new Vector2(0, -size));
        points.Add(new Vector2(0, 0));
        points.Add(new Vector2(-size * 0.7f, size * 0.7f));
        points.Add(new Vector2(size * 0.7f, -size * 0.7f));
        points.Add(new Vector2(0, 0));
        points.Add(new Vector2(size * 0.7f, size * 0.7f));
        points.Add(new Vector2(-size * 0.7f, -size * 0.7f));
        int circleStart = points.Count;
        for (int i = 0; i <= 32; i++)
        {
            float angle = i * Mathf.PI * 2f / 32;
            points.Add(new Vector2(Mathf.Cos(angle) * size, Mathf.Sin(angle) * size));
        }
    }

    void GenerateInfinityPreview(List<Vector2> points, float size)
    {
        int pointCount = 64;
        for (int i = 0; i <= pointCount; i++)
        {
            float t = i / (float)pointCount * Mathf.PI * 2f;
            float x = Mathf.Sin(t) * size;
            float y = Mathf.Sin(t) * Mathf.Cos(t) * size * 0.5f;
            points.Add(new Vector2(x, y));
        }
    }

    void GenerateComplexRunePreview(List<Vector2> points, float radius)
    {
        GenerateCirclePreview(points, radius, 24);
        int pointCount = points.Count;
        for (int i = 0; i < 8; i++)
        {
            float angle = i * Mathf.PI * 2f / 8f;
            points.Add(new Vector2(Mathf.Cos(angle) * radius * 0.5f, Mathf.Sin(angle) * radius * 0.5f));
        }
        points.Add(points[pointCount]);
    }
#endif
}
