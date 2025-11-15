using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 게임 전체 로직 관리
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("References")]
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

    private const int TOTAL_STAGES = 12; // 전체 스테이지 개수
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

        // 시간 초과
        if (remainingTime <= 0)
        {
            OnMagicComplete();
        }

        // 승리 조건 체크: 모든 약점을 파괴해야 함
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
        remainingTime = castTime;

        // 이전 마법진 제거
        if (currentCircle != null)
        {
            Destroy(currentCircle.gameObject);
        }

        // 새 마법진 생성
        currentCircle = Instantiate(magicCirclePrefab, spawnPosition, Quaternion.identity);

        // 난이도에 따라 패턴 변경
        currentCircle.pattern = GetPatternForStage(currentStage);
        currentCircle.drawDuration = castTime;
        currentCircle.StartDrawing();

        // SlashDetector에 타겟 설정
        if (slashDetector != null)
        {
            slashDetector.SetTarget(currentCircle);
        }

        UpdateUI();
        Debug.Log($"Stage {currentStage} started!");
    }

    MagicCircle.CirclePattern GetPatternForStage(int stage)
    {
        switch (stage)
        {
            case 1: return MagicCircle.CirclePattern.Circle;
            case 2: return MagicCircle.CirclePattern.Triangle;
            case 3: return MagicCircle.CirclePattern.Square;
            case 4: return MagicCircle.CirclePattern.Pentagram;
            case 5: return MagicCircle.CirclePattern.Hexagram;
            case 6: return MagicCircle.CirclePattern.Heptagram;
            case 7: return MagicCircle.CirclePattern.Octagram;
            case 8: return MagicCircle.CirclePattern.DoublePentagram;
            case 9: return MagicCircle.CirclePattern.CrossPattern;
            case 10: return MagicCircle.CirclePattern.Spiral;
            case 11: return MagicCircle.CirclePattern.InfinitySymbol;
            default: return MagicCircle.CirclePattern.ComplexRune;
        }
    }

    void OnPlayerWin()
    {
        isGameActive = false;
        int stageScore = Mathf.RoundToInt(remainingTime * 100);
        totalScore += stageScore;

        if (statusText != null)
        {
            statusText.text = $"STAGE {currentStage} CLEAR! +{stageScore} points";
            statusText.color = Color.green;
        }

        Debug.Log($"Stage {currentStage} cleared! Score: {stageScore}");

        // 마법진 파괴
        currentCircle.Destroy();

        // 전체 스테이지 클리어 체크
        if (currentStage >= TOTAL_STAGES)
        {
            Invoke(nameof(OnGameComplete), 2f);
        }
        else
        {
            // 다음 스테이지
            Invoke(nameof(StartNextStage), 2f);
        }
    }

    void OnMagicComplete()
    {
        isGameActive = false;

        if (statusText != null)
        {
            statusText.text = $"STAGE {currentStage} FAILED! Retry...";
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
            statusText.text = $"ALL STAGES CLEAR!\nFinal Score: {totalScore}";
            statusText.color = Color.yellow;
            statusText.fontSize = 40;
        }

        if (timerText != null)
        {
            timerText.text = "CONGRATULATIONS!";
        }

        Debug.Log($"All stages cleared! Final Score: {totalScore}");

        // 5초 후 게임 재시작
        Invoke(nameof(RestartGame), 5f);
    }

    void UpdateUI()
    {
        if (timerText != null)
        {
            timerText.text = $"Time: {remainingTime:F1}s";
        }

        if (scoreText != null)
        {
            scoreText.text = $"Score: {totalScore}";
        }

        if (statusText != null && isGameActive)
        {
            int brokenWeakpoints = currentCircle != null ? currentCircle.GetBrokenWeakpointCount() : 0;
            int totalWeakpoints = currentCircle != null ? currentCircle.GetTotalWeakpointCount() : 0;
            statusText.text = $"Stage {currentStage}/{TOTAL_STAGES} - Destroy {brokenWeakpoints}/{totalWeakpoints} weakpoints!";
            statusText.color = Color.white;
            statusText.fontSize = 32; // 기본 크기로 복원
        }
    }
}
