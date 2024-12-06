using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Major stats 主要数值")]
    public Stat strength; // 力量：增加基础伤害和暴击伤害
    public Stat agility; // 敏捷：增加闪避率和暴击率
    public Stat intelligence; // 智慧：增加魔法伤害和魔法抗性
    public Stat vitality; // 活力：增加最大生命值
    
    [Header("Defensive stats 防御数值")]
    public Stat maxHealth;
    public Stat armor; // 防御力：减少伤害
    public Stat evasion; // 闪避值（灵敏度）
    
    public Stat damage;

    [SerializeField]private int currentHealth;

    protected virtual void Start()
    {
        currentHealth = maxHealth.GetValue();
    }

    public virtual void DoDamage(CharacterStats _targetStats)
    {
        if (TargetCanAvoidAttack(_targetStats))
            return;

        TargetCanAvoidAttack(_targetStats);
        int totalDamage = damage.GetValue() + strength.GetValue();
        
        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        _targetStats.TakeDamage(totalDamage);
    }
    
    public virtual void TakeDamage(int _damage)
    {
        currentHealth -= _damage;

        Debug.Log(_damage);
        
        if (currentHealth < 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        // throw new NotImplementedException();
    }
    private bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + agility.GetValue();

        if (Random.Range(0, 100) < totalEvasion)
        {
            return true;
        }
        return false;
    }
    private int CheckTargetArmor(CharacterStats _targetStats, int totalDamage)
    {
        totalDamage -= _targetStats.armor.GetValue();
        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }
}
