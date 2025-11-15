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
            statusText.text = $"SUCCESS! +{stageScore} points";
            statusText.color = Color.green;
        }

        Debug.Log($"Player wins! Score: {stageScore}");

        // 마법진 파괴
        currentCircle.Destroy();

        // 다음 스테이지
        Invoke(nameof(StartNextStage), 2f);
    }

    void OnMagicComplete()
    {
        isGameActive = false;

        if (statusText != null)
        {
            statusText.text = "MAGIC ACTIVATED! You failed...";
            statusText.color = Color.red;
        }

        Debug.Log("Player failed! Magic activated.");

        // 게임 재시작
        Invoke(nameof(RestartGame), 2f);
    }

    void RestartGame()
    {
        currentStage = 0;
        totalScore = 0;
        StartNextStage();
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
            statusText.text = $"Stage {currentStage} - Destroy {brokenWeakpoints}/{totalWeakpoints} weakpoints!";
            statusText.color = Color.white;
        }
    }
}
