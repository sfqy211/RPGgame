using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Major stats 主要数值")] 
    public Stat strength; // 力量：增加基础伤害和暴击伤害
    public Stat agility; // 敏捷：增加闪避率和暴击率
    public Stat intelligence; // 智慧：增加魔法伤害和魔法抗性
    public Stat vitality; // 活力：增加最大生命值

    [Header("Offensive stats 攻击数值")] 
    public Stat damage; // 基础伤害
    public Stat critChance; // 暴击率
    public Stat critPower; // 暴击倍率

    [Header("Defensive stats 防御数值")] 
    public Stat maxHealth; // 最大生命值
    public Stat armor; // 防御力：减少伤害
    public Stat evasion; // 闪避值（灵敏度）
    public Stat magicResistance; // 魔法抗性

    [Header("Magic stats 魔法伤害")] 
    public Stat fireDamage; // 火元素伤害
    public Stat iceDamage; // 冰元素伤害
    public Stat lightningDamage; // 电元素伤害

    [Header("Ailments 状态")]
    public bool isIgnited; // 点燃
    public bool isChilled; // 冰冻，减少20%护甲
    public bool isShocked; // 电击，减少20%准确度

    private float ignitedTimer;
    private float chilledTimer;
    private float shockedTimer;
    
    private float ignitedDamageCooldown = .3f;
    private float ignitedDamageTimer;
    private int ignitedDamage;

    public int currentHealth;

    public System.Action onHealthChanged;

    protected virtual void Start()
    {
        critPower.SetDefaultValue(150); // 暴击倍率默认为1.5倍
        currentHealth = GetMaxHealthValue();
    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;
        
        ignitedDamageTimer -= Time.deltaTime;
        
        if(ignitedTimer < 0)
            isIgnited = false;
        if(chilledTimer < 0)
            isChilled = false;
        if(shockedTimer < 0)
            isShocked = false;
        
        if(ignitedDamageTimer < 0 && isIgnited)
        {
            Debug.Log("受到燃烧伤害" + ignitedDamage);
            
            DecreaseHealthBy(ignitedDamage);
            
            currentHealth -= ignitedDamage;
            if(currentHealth < 0)
                Die();
            ignitedDamageTimer = ignitedDamageCooldown;
        }
    }

    public virtual void DoDamage(CharacterStats _targetStats)
    {
        if (TargetCanAvoidAttack(_targetStats))
            return;

        int totalDamage = damage.GetValue() + strength.GetValue();

        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
        }

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        _targetStats.TakeDamage(totalDamage);
        DoMagicalDamage(_targetStats);
    }

    public virtual void DoMagicalDamage(CharacterStats _targetStats)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightningDamage = lightningDamage.GetValue();

        int totalMagicDamage = _fireDamage + _iceDamage + _lightningDamage + intelligence.GetValue();

        totalMagicDamage = CheckTargetResistance(_targetStats, totalMagicDamage);
        _targetStats.TakeDamage(totalMagicDamage);

        if (Mathf.Max(_fireDamage, _iceDamage, _lightningDamage) <= 0)
            return;

        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightningDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightningDamage;
        bool canApplyShock = _lightningDamage > _fireDamage && _lightningDamage > _iceDamage;

        while (!canApplyChill && !canApplyShock && !canApplyIgnite)
        {
            if (Random.value < .3f && _fireDamage > 0)
            {
                canApplyIgnite = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                Debug.Log("点燃");
                return;
            }
            if (Random.value < .3f && _iceDamage > 0)
            {
                canApplyChill = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                Debug.Log("冰冻");
                return;
            }
            if (Random.value < .3f && _lightningDamage > 0)
            {
                canApplyShock = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                Debug.Log("电击");
                return;
            }
        }
        if(canApplyIgnite)
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * .2f));
        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
    }

    private static int CheckTargetResistance(CharacterStats _targetStats, int _totalMagicDamage)
    {
        _totalMagicDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);
        _totalMagicDamage = Mathf.Clamp(_totalMagicDamage, 0, int.MaxValue);
        return _totalMagicDamage;
    }

    public void ApplyAilments(bool _ignite, bool _chill, bool _shock)
    {
        if (isIgnited || isChilled || isShocked)
            return; // 保证只有一个状态存在

        if(_ignite)
        {
            isIgnited = _ignite;
            ignitedTimer = 2;
        }
        if (_chill)
        {
            isChilled = _chill;
            chilledTimer = 2;
        }
        if (_shock)
        {
            isShocked = _shock;
            shockedTimer = 2;
        }
    }

    public void SetupIgniteDamage(int _damage) => ignitedDamage = _damage;
    
    protected virtual void TakeDamage(int _damage)
    {
        DecreaseHealthBy(_damage);

        Debug.Log(_damage);

        if (currentHealth < 0)
            Die();
    }

    protected virtual void DecreaseHealthBy(int _damage)
    {
        currentHealth -= _damage;
        if (onHealthChanged != null)
            onHealthChanged();
    }

    protected virtual void Die()
    {
        // throw new NotImplementedException();
    }

    private bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + agility.GetValue();

        if (isShocked)
            totalEvasion += 20;
        if (Random.Range(0, 100) < totalEvasion)
            return true;

        return false;
    }

    private int CheckTargetArmor(CharacterStats _targetStats, int _totalDamage)
    {
        if(_targetStats.isChilled)
            _totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * .8f);
        else
            _totalDamage -= _targetStats.armor.GetValue();
        
        _totalDamage = Mathf.Clamp(_totalDamage, 0, int.MaxValue);
        return _totalDamage;
    }

    private bool CanCrit()
    {
        int totalCriticalChance = critChance.GetValue() + agility.GetValue();
        if (Random.Range(0, 100) <= totalCriticalChance)
            return true;
        return false;
    }

    private int CalculateCriticalDamage(int _damage)
    {
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * .01f;
        float critDamage = _damage * totalCritPower;
        return Mathf.RoundToInt(critDamage);
    }

    public int GetMaxHealthValue() => maxHealth.GetValue() + vitality.GetValue() * 5;
    
}