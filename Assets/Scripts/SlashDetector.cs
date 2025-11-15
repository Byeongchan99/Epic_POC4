using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 마우스 드래그로 마법진 자르기
/// </summary>
public class SlashDetector : MonoBehaviour
{
    [Header("Slash Settings")]
    public float slashWidth = 0.15f;
    public Color slashColor = new Color(1f, 0.3f, 0.3f, 1f);
    public float trailDuration = 0.3f;

    [Header("Effects")]
    public GameObject cutEffectPrefab;

    private Camera mainCamera;
    private LineRenderer trailRenderer;
    private List<Vector2> slashPoints = new List<Vector2>();
    private Vector2 lastSlashPoint;
    private bool isSlashing = false;
    private MagicCircle currentTarget;

    void Awake()
    {
        mainCamera = Camera.main;
        SetupTrailRenderer();
    }

    void SetupTrailRenderer()
    {
        GameObject trailObj = new GameObject("SlashTrail");
        trailObj.transform.parent = transform;
        trailRenderer = trailObj.AddComponent<LineRenderer>();

        trailRenderer.material = new Material(Shader.Find("Sprites/Default"));
        trailRenderer.startColor = slashColor;
        trailRenderer.endColor = new Color(slashColor.r, slashColor.g, slashColor.b, 0f);
        trailRenderer.startWidth = slashWidth;
        trailRenderer.endWidth = slashWidth * 0.5f;
        trailRenderer.useWorldSpace = true;
        trailRenderer.sortingOrder = 10;
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        // 마우스 클릭 시작
        if (Input.GetMouseButtonDown(0))
        {
            StartSlash();
        }

        // 드래그 중
        if (Input.GetMouseButton(0) && isSlashing)
        {
            UpdateSlash();
        }

        // 마우스 떼기
        if (Input.GetMouseButtonUp(0))
        {
            EndSlash();
        }
    }

    void StartSlash()
    {
        isSlashing = true;
        slashPoints.Clear();

        Vector2 worldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        slashPoints.Add(worldPos);
        lastSlashPoint = worldPos;

        trailRenderer.positionCount = 1;
        trailRenderer.SetPosition(0, worldPos);
    }

    void UpdateSlash()
    {
        Vector2 worldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // 일정 거리 이상 움직였을 때만 추가
        if (Vector2.Distance(worldPos, lastSlashPoint) > 0.05f)
        {
            slashPoints.Add(worldPos);
            lastSlashPoint = worldPos;

            // 트레일 업데이트
            trailRenderer.positionCount = slashPoints.Count;
            trailRenderer.SetPosition(slashPoints.Count - 1, worldPos);

            // 교차 판정
            if (slashPoints.Count >= 2)
            {
                CheckIntersections(slashPoints[slashPoints.Count - 2], worldPos);
            }
        }
    }

    void EndSlash()
    {
        isSlashing = false;

        // 트레일 페이드 아웃
        StartCoroutine(FadeOutTrail());
    }

    System.Collections.IEnumerator FadeOutTrail()
    {
        yield return new WaitForSeconds(trailDuration);
        trailRenderer.positionCount = 0;
        slashPoints.Clear();
    }

    /// <summary>
    /// 마법진과의 교차 체크
    /// </summary>
    void CheckIntersections(Vector2 slashStart, Vector2 slashEnd)
    {
        if (currentTarget == null) return;

        var segments = currentTarget.Segments;
        for (int i = 0; i < segments.Count; i++)
        {
            if (segments[i].isBroken) continue;

            if (LineSegment.Intersects(slashStart, slashEnd, segments[i].start, segments[i].end))
            {
                // 선분 끊기 시도 (그려진 선분만 잘림)
                bool wasCut = currentTarget.BreakSegment(i);

                // 실제로 잘렸을 때만 이펙트 생성
                if (wasCut)
                {
                    // 교차점 계산
                    Vector2 intersectionPoint = LineSegment.GetIntersectionPoint(
                        slashStart, slashEnd,
                        segments[i].start, segments[i].end
                    );

                    // 효과 생성
                    SpawnCutEffect(intersectionPoint);

                    Debug.Log($"Cut! Segment {i} at {intersectionPoint}");
                }
                else
                {
                    // 그려지지 않은 선분을 잘라려고 시도
                    Debug.Log($"Cannot cut segment {i} - not fully drawn yet");
                }
            }
        }
    }

    /// <summary>
    /// 자르기 효과 생성
    /// </summary>
    void SpawnCutEffect(Vector2 position)
    {
        // 파티클 효과
        if (cutEffectPrefab != null)
        {
            Instantiate(cutEffectPrefab, position, Quaternion.identity);
        }
        else
        {
            // cutEffectPrefab이 없으면 런타임에 생성
            GameObject effectObj = new GameObject("CutEffect");
            effectObj.transform.position = position;
            CutEffect effect = effectObj.AddComponent<CutEffect>();
            effect.particleColor = new Color(1f, 0.8f, 0.2f);
            effect.particleCount = 15;
            effect.explosionForce = 2f;
        }

        // 화면 쉐이크 (간단한 버전)
        StartCoroutine(CameraShake());
    }

    System.Collections.IEnumerator CameraShake()
    {
        Vector3 originalPos = mainCamera.transform.position;
        float duration = 0.1f;
        float magnitude = 0.1f;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            mainCamera.transform.position = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = originalPos;
    }

    /// <summary>
    /// 현재 타겟 마법진 설정
    /// </summary>
    public void SetTarget(MagicCircle target)
    {
        currentTarget = target;
    }
}
