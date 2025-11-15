using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 게임 전체 로직 관리
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("References")]
    [Tooltip("(Deprecated) Fallback용. 이제 Stage Settings의 Stage Prefabs를 사용합니다.")]
    public MagicCircle magicCirclePrefab;
    public SlashDetector slashDetector;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;

    [Header("Game Settings")]
    public float castTime = 3f;
    [Tooltip("(Deprecated) 이제 약점 시스템 사용 - MagicCircle의 weakpointRatio 참고")]
    public int cutsNeededToWin = 3; // 더 이상 사용하지 않음
    public Vector2 spawnPosition = Vector2.zero;

    [Header("Stage Settings")]
    [Tooltip("각 스테이지에서 사용할 MagicCircle 프리팹을 설정합니다. 배열 크기가 전체 스테이지 개수가 됩니다.")]
    public MagicCircle[] stagePrefabs;

    private int TotalStages => stagePrefabs != null && stagePrefabs.Length > 0 ? stagePrefabs.Length : 1;
    private MagicCircle currentCircle;
    private float remainingTime;
    private int currentStage = 0;
    private int totalScore = 0;
    private bool isGameActive = false;

    void Start()
    {
        StartNextStage();
    }

    void Update()
    {
        if (!isGameActive) return;

        // 타이머 업데이트
        remainingTime -= Time.deltaTime;
        UpdateUI();

        // 실패 조건 1: 보라색 선분(일반 선분)을 자른 경우
        if (currentCircle != null && currentCircle.HasBrokenNormalSegment())
        {
            OnPlayerFail();
            return;
        }

        // 실패 조건 2: 시간 초과
        if (remainingTime <= 0)
        {
            OnMagicComplete();
            return;
        }

        // 승리 조건: 모든 약점을 파괴 + 일반 선분은 건드리지 않음
        if (currentCircle != null &&
            currentCircle.GetBrokenWeakpointCount() >= currentCircle.GetTotalWeakpointCount())
        {
            OnPlayerWin();
        }
    }

    void StartNextStage()
    {
        currentStage++;
        isGameActive = true;

        // 이전 마법진 제거
        if (currentCircle != null)
        {
            Destroy(currentCircle.gameObject);
        }

        // 스테이지에 맞는 프리팹 가져오기
        MagicCircle prefabToUse = GetPrefabForStage(currentStage);

        if (prefabToUse == null)
        {
            Debug.LogError($"Stage {currentStage}: 프리팹이 할당되지 않았습니다!");
            return;
        }

        // 새 마법진 생성 (프리팹의 모든 설정 그대로 사용)
        currentCircle = Instantiate(prefabToUse, spawnPosition, Quaternion.identity);

        // 그리기 시작 (프리팹의 drawDuration 사용)
        currentCircle.StartDrawing();

        // 스테이지 제한 시간 설정 (프리팹의 stageTimeLimit 또는 기본 castTime)
        if (currentCircle.stageTimeLimit > 0)
        {
            remainingTime = currentCircle.stageTimeLimit;
        }
        else
        {
            remainingTime = castTime; // 기본값 사용
        }

        // SlashDetector에 타겟 설정
        if (slashDetector != null)
        {
            slashDetector.SetTarget(currentCircle);
        }

        UpdateUI();
        Debug.Log($"Stage {currentStage} started with prefab: {prefabToUse.name}, drawDuration: {currentCircle.drawDuration}s, stageTime: {remainingTime}s");
    }

    MagicCircle GetPrefabForStage(int stage)
    {
        // 스테이지는 1부터 시작하므로 배열 인덱스는 stage - 1
        int index = stage - 1;

        if (stagePrefabs != null && index >= 0 && index < stagePrefabs.Length)
        {
            return stagePrefabs[index];
        }

        // Fallback: 기본 프리팹 사용
        if (magicCirclePrefab != null)
        {
            Debug.LogWarning($"Stage {stage}: stagePrefabs가 비어있어 fallback 프리팹 사용");
            return magicCirclePrefab;
        }

        return null;
    }

    void OnPlayerWin()
    {
        isGameActive = false;
        int stageScore = Mathf.RoundToInt(remainingTime * 100);
        totalScore += stageScore;

        if (statusText != null)
        {
            statusText.text = $"스테이지 {currentStage} 클리어! +{stageScore}점";
            statusText.color = Color.green;
        }

        Debug.Log($"Stage {currentStage} cleared! Score: {stageScore}");

        // 마법진 파괴
        currentCircle.Destroy();

        // 전체 스테이지 클리어 체크
        if (currentStage >= TotalStages)
        {
            Invoke(nameof(OnGameComplete), 2f);
        }
        else
        {
            // 다음 스테이지
            Invoke(nameof(StartNextStage), 2f);
        }
    }

    void OnPlayerFail()
    {
        isGameActive = false;

        if (statusText != null)
        {
            statusText.text = $"다른 마력선을 자르지 마세요!";
            statusText.color = Color.red;
        }

        Debug.Log($"Stage {currentStage} failed! Cut normal segment (purple).");

        // 현재 스테이지 재시작
        Invoke(nameof(RestartStage), 2f);
    }

    void OnMagicComplete()
    {
        isGameActive = false;

        if (statusText != null)
        {
            statusText.text = $"시간 초과!";
            statusText.color = Color.red;
        }

        Debug.Log($"Stage {currentStage} failed! Magic activated.");

        // 현재 스테이지 재시작
        Invoke(nameof(RestartStage), 2f);
    }

    /// <summary>
    /// 현재 스테이지 재시작
    /// </summary>
    void RestartStage()
    {
        // 스테이지는 유지, 점수도 유지
        currentStage--; // StartNextStage에서 ++되므로 미리 감소
        StartNextStage();
    }

    /// <summary>
    /// 전체 게임 재시작 (처음부터)
    /// </summary>
    void RestartGame()
    {
        currentStage = 0;
        totalScore = 0;
        StartNextStage();
    }

    /// <summary>
    /// 전체 게임 클리어
    /// </summary>
    void OnGameComplete()
    {
        if (statusText != null)
        {
            statusText.text = $"모든 스테이지 클리어!\n최종 점수: {totalScore}";
            statusText.color = Color.yellow;
            statusText.fontSize = 40;
        }

        if (timerText != null)
        {
            timerText.text = "축하합니다!";
        }

        Debug.Log($"All stages cleared! Final Score: {totalScore}");

        // 5초 후 게임 재시작
        Invoke(nameof(RestartGame), 5f);
    }

    void UpdateUI()
    {
        if (timerText != null)
        {
            timerText.text = $"남은 시간: {remainingTime:F1}s";
        }

        if (scoreText != null)
        {
            scoreText.text = $"점수: {totalScore}";
        }

        if (statusText != null && isGameActive)
        {
            int brokenWeakpoints = currentCircle != null ? currentCircle.GetBrokenWeakpointCount() : 0;
            int totalWeakpoints = currentCircle != null ? currentCircle.GetTotalWeakpointCount() : 0;
            statusText.text = $"스테이지 {currentStage}/{TotalStages} - 잘라야 할 마력선의 개쉬: {brokenWeakpoints}/{totalWeakpoints}";
            statusText.color = Color.white;
            statusText.fontSize = 32; // 기본 크기로 복원
        }
    }
}
