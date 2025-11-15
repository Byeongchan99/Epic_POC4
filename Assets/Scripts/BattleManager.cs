using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 턴제 전투 관리자
/// </summary>
public class BattleManager : MonoBehaviour
{
    [Header("Characters")]
    public BattleCharacter player;
    public BattleCharacter enemy;

    [Header("Spell Prefabs")]
    [Tooltip("공격 마법진 프리팹 리스트")]
    public List<MagicCircle> attackSpellPrefabs = new List<MagicCircle>();
    [Tooltip("방어 마법진 프리팹 리스트")]
    public List<MagicCircle> defenseSpellPrefabs = new List<MagicCircle>();

    [Header("Spell Settings")]
    [Tooltip("마법진 생성 위치")]
    public Vector3 spellSpawnPosition = Vector3.zero;
    [Tooltip("마법진 파괴 제한 시간")]
    public float spellTimeLimit = 15f;

    [Header("UI")]
    public BattleUI battleUI;

    private BattleState currentState = BattleState.PlayerTurn;
    private MagicCircle currentSpell;
    private bool isProcessingSpell = false;
    private float spellTimer = 0f;

    public enum BattleState
    {
        PlayerTurn,
        EnemyTurn,
        BattleEnd
    }

    void Start()
    {
        StartBattle();
    }

    void Update()
    {
        // 마법진 처리 중일 때 타이머 체크
        if (isProcessingSpell && currentSpell != null)
        {
            spellTimer -= Time.deltaTime;

            // 시간 초과
            if (spellTimer <= 0f)
            {
                OnSpellTimeOut();
            }
        }
    }

    /// <summary>
    /// 전투 시작
    /// </summary>
    void StartBattle()
    {
        battleUI.UpdatePlayerHP(player.CurrentHP, player.maxHP);
        battleUI.UpdateEnemyHP(enemy.CurrentHP, enemy.maxHP);
        battleUI.ShowMessage($"전투 시작! {enemy.characterName}와의 대결!");

        StartPlayerTurn();
    }

    /// <summary>
    /// 플레이어 턴 시작
    /// </summary>
    void StartPlayerTurn()
    {
        currentState = BattleState.PlayerTurn;
        battleUI.ShowMessage("당신의 턴입니다.");
        battleUI.SetAttackButtonActive(true);
        battleUI.SetEndTurnButtonActive(false);
    }

    /// <summary>
    /// 공격 버튼 클릭 (플레이어 턴)
    /// </summary>
    public void OnAttackButtonClicked()
    {
        if (currentState != BattleState.PlayerTurn || isProcessingSpell) return;

        battleUI.SetAttackButtonActive(false);
        battleUI.ShowMessage("상대방이 방어 마법 사용!");

        // 적의 방어 마법 생성
        SpawnDefenseSpell();
    }

    /// <summary>
    /// 턴 종료 버튼 클릭
    /// </summary>
    public void OnEndTurnButtonClicked()
    {
        if (isProcessingSpell) return;

        battleUI.SetEndTurnButtonActive(false);

        if (currentState == BattleState.PlayerTurn)
        {
            StartEnemyTurn();
        }
        else if (currentState == BattleState.EnemyTurn)
        {
            if (CheckBattleEnd()) return;
            StartPlayerTurn();
        }
    }

    /// <summary>
    /// 적 턴 시작
    /// </summary>
    void StartEnemyTurn()
    {
        currentState = BattleState.EnemyTurn;
        battleUI.ShowMessage("상대방의 턴입니다.");

        // 약간의 딜레이 후 적의 공격 마법 생성
        StartCoroutine(EnemyAttackCoroutine());
    }

    IEnumerator EnemyAttackCoroutine()
    {
        yield return new WaitForSeconds(1f);

        battleUI.ShowMessage("상대방이 공격 마법 사용!");
        SpawnAttackSpell();
    }

    /// <summary>
    /// 방어 마법 생성 (플레이어가 공격할 때)
    /// </summary>
    void SpawnDefenseSpell()
    {
        if (defenseSpellPrefabs.Count == 0)
        {
            Debug.LogError("방어 마법진 프리팹이 없습니다!");
            return;
        }

        // 랜덤으로 방어 마법 선택
        MagicCircle prefab = defenseSpellPrefabs[Random.Range(0, defenseSpellPrefabs.Count)];
        currentSpell = Instantiate(prefab, spellSpawnPosition, Quaternion.identity);

        // 마법진 그리기 시작
        currentSpell.StartDrawing();

        isProcessingSpell = true;
        spellTimer = spellTimeLimit;

        // 마법진 완료 체크 시작
        StartCoroutine(CheckSpellCompletion(true));
    }

    /// <summary>
    /// 공격 마법 생성 (적이 공격할 때)
    /// </summary>
    void SpawnAttackSpell()
    {
        if (attackSpellPrefabs.Count == 0)
        {
            Debug.LogError("공격 마법진 프리팹이 없습니다!");
            return;
        }

        // 랜덤으로 공격 마법 선택
        MagicCircle prefab = attackSpellPrefabs[Random.Range(0, attackSpellPrefabs.Count)];
        currentSpell = Instantiate(prefab, spellSpawnPosition, Quaternion.identity);

        // 마법진 그리기 시작
        currentSpell.StartDrawing();

        isProcessingSpell = true;
        spellTimer = spellTimeLimit;

        // 마법진 완료 체크 시작
        StartCoroutine(CheckSpellCompletion(false));
    }

    /// <summary>
    /// 마법진 파괴 완료 체크
    /// </summary>
    IEnumerator CheckSpellCompletion(bool isDefenseSpell)
    {
        while (isProcessingSpell && currentSpell != null)
        {
            // 보라색 선분을 자른 경우 즉시 실패
            if (currentSpell.HasBrokenNormalSegment())
            {
                OnSpellFailed(isDefenseSpell, true);
                yield break;
            }

            // 모든 약점을 파괴한 경우 성공
            if (currentSpell.GetBrokenWeakpointCount() >= currentSpell.GetTotalWeakpointCount())
            {
                OnSpellSuccess(isDefenseSpell);
                yield break;
            }

            yield return null;
        }
    }

    /// <summary>
    /// 마법진 파괴 성공
    /// </summary>
    void OnSpellSuccess(bool isDefenseSpell)
    {
        isProcessingSpell = false;

        if (isDefenseSpell)
        {
            // 방어 마법 파괴 성공 = 적에게 데미지
            int damage = player.GetTotalAttackPower(currentSpell.attackPower);
            enemy.TakeDamage(damage);

            battleUI.ShowMessage($"방어 마법을 완전히 파괴! {enemy.characterName}에게 {damage} 데미지!");
            battleUI.UpdateEnemyHP(enemy.CurrentHP, enemy.maxHP);
        }
        else
        {
            // 공격 마법 파괴 성공 = 플레이어 데미지 0
            battleUI.ShowMessage("공격 마법을 완전히 파괴하여 데미지를 받지 않았습니다!");
        }

        DestroyCurrentSpell();
        battleUI.SetEndTurnButtonActive(true);
    }

    /// <summary>
    /// 마법진 파괴 실패
    /// </summary>
    void OnSpellFailed(bool isDefenseSpell, bool hitPurple)
    {
        isProcessingSpell = false;

        if (isDefenseSpell)
        {
            // 방어 마법 파괴 실패 = 적에게 데미지 없음
            if (hitPurple)
                battleUI.ShowMessage("보라색 선분을 자름! 공격 실패!");
            else
                battleUI.ShowMessage("시간 초과! 공격 실패!");
        }
        else
        {
            // 공격 마법 파괴 실패 = 플레이어 데미지
            int damage = enemy.GetTotalAttackPower(currentSpell.attackPower);
            player.TakeDamage(damage);

            if (hitPurple)
                battleUI.ShowMessage($"보라색 선분을 자름! {damage} 데미지를 받았습니다!");
            else
                battleUI.ShowMessage($"시간 초과! {damage} 데미지를 받았습니다!");

            battleUI.UpdatePlayerHP(player.CurrentHP, player.maxHP);
        }

        DestroyCurrentSpell();
        battleUI.SetEndTurnButtonActive(true);
    }

    /// <summary>
    /// 마법진 시간 초과
    /// </summary>
    void OnSpellTimeOut()
    {
        if (!isProcessingSpell) return;

        bool isDefenseSpell = (currentState == BattleState.PlayerTurn);
        OnSpellFailed(isDefenseSpell, false);
    }

    /// <summary>
    /// 현재 마법진 제거
    /// </summary>
    void DestroyCurrentSpell()
    {
        if (currentSpell != null)
        {
            Destroy(currentSpell.gameObject);
            currentSpell = null;
        }
    }

    /// <summary>
    /// 전투 종료 체크
    /// </summary>
    bool CheckBattleEnd()
    {
        if (enemy.IsDead)
        {
            OnPlayerWin();
            return true;
        }

        if (player.IsDead)
        {
            OnPlayerLose();
            return true;
        }

        return false;
    }

    /// <summary>
    /// 플레이어 승리
    /// </summary>
    void OnPlayerWin()
    {
        currentState = BattleState.BattleEnd;
        battleUI.ShowMessage($"승리! {enemy.characterName}를 물리쳤습니다!");
        battleUI.SetAttackButtonActive(false);
        battleUI.SetEndTurnButtonActive(false);
    }

    /// <summary>
    /// 플레이어 패배
    /// </summary>
    void OnPlayerLose()
    {
        currentState = BattleState.BattleEnd;
        battleUI.ShowMessage($"패배... {enemy.characterName}에게 졌습니다.");
        battleUI.SetAttackButtonActive(false);
        battleUI.SetEndTurnButtonActive(false);
    }
}
