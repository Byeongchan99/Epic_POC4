using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 전투 UI 관리
/// </summary>
public class BattleUI : MonoBehaviour
{
    [Header("Player UI (왼쪽 아래)")]
    public TextMeshProUGUI playerNameText;
    public Slider playerHPSlider;
    public TextMeshProUGUI playerHPText;

    [Header("Enemy UI (오른쪽 위)")]
    public TextMeshProUGUI enemyNameText;
    public Slider enemyHPSlider;
    public TextMeshProUGUI enemyHPText;

    [Header("Info Panel")]
    public TextMeshProUGUI messageText;

    [Header("Buttons")]
    public Button attackButton;
    public Button endTurnButton;

    void Start()
    {
        // 버튼 리스너 연결
        BattleManager battleManager = FindObjectOfType<BattleManager>();
        if (battleManager != null)
        {
            attackButton.onClick.AddListener(battleManager.OnAttackButtonClicked);
            endTurnButton.onClick.AddListener(battleManager.OnEndTurnButtonClicked);
        }
    }

    /// <summary>
    /// 플레이어 체력 업데이트
    /// </summary>
    public void UpdatePlayerHP(int current, int max)
    {
        if (playerHPSlider != null)
        {
            playerHPSlider.maxValue = max;
            playerHPSlider.value = current;
        }

        if (playerHPText != null)
        {
            playerHPText.text = $"{current} / {max}";
        }
    }

    /// <summary>
    /// 적 체력 업데이트
    /// </summary>
    public void UpdateEnemyHP(int current, int max)
    {
        if (enemyHPSlider != null)
        {
            enemyHPSlider.maxValue = max;
            enemyHPSlider.value = current;
        }

        if (enemyHPText != null)
        {
            enemyHPText.text = $"{current} / {max}";
        }
    }

    /// <summary>
    /// 메시지 표시
    /// </summary>
    public void ShowMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
        }
    }

    /// <summary>
    /// 공격 버튼 활성화/비활성화
    /// </summary>
    public void SetAttackButtonActive(bool active)
    {
        if (attackButton != null)
        {
            attackButton.interactable = active;
        }
    }

    /// <summary>
    /// 턴 종료 버튼 활성화/비활성화
    /// </summary>
    public void SetEndTurnButtonActive(bool active)
    {
        if (endTurnButton != null)
        {
            endTurnButton.interactable = active;
        }
    }

    /// <summary>
    /// 캐릭터 이름 설정
    /// </summary>
    public void SetCharacterNames(string playerName, string enemyName)
    {
        if (playerNameText != null)
            playerNameText.text = playerName;

        if (enemyNameText != null)
            enemyNameText.text = enemyName;
    }
}
