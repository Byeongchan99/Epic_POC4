using UnityEngine;

/// <summary>
/// 전투 캐릭터 (플레이어/적 공통)
/// </summary>
public class BattleCharacter : MonoBehaviour
{
    [Header("Character Stats")]
    [Tooltip("최대 체력")]
    public int maxHP = 100;
    [Tooltip("기본 공격력 (마법진 공격력에 더해짐)")]
    public int baseAttackPower = 0;

    [Header("Character Info")]
    public string characterName = "Character";

    private int currentHP;

    public int CurrentHP => currentHP;
    public bool IsDead => currentHP <= 0;

    void Awake()
    {
        currentHP = maxHP;
    }

    /// <summary>
    /// 데미지를 받습니다
    /// </summary>
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP < 0) currentHP = 0;
    }

    /// <summary>
    /// 체력을 회복합니다
    /// </summary>
    public void Heal(int amount)
    {
        currentHP += amount;
        if (currentHP > maxHP) currentHP = maxHP;
    }

    /// <summary>
    /// 체력을 최대치로 회복합니다
    /// </summary>
    public void FullHeal()
    {
        currentHP = maxHP;
    }

    /// <summary>
    /// 총 공격력을 계산합니다 (기본 공격력 + 마법진 공격력)
    /// </summary>
    public int GetTotalAttackPower(int spellAttackPower)
    {
        return baseAttackPower + spellAttackPower;
    }
}
