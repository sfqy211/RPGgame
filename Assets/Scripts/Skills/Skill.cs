using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    [SerializeField] private float cooldown;
    protected float CooldownTimer;
    
    protected virtual void Start()
    {
        CooldownTimer -= Time.deltaTime;
    }

    public virtual bool CanUseSkill()
    {
        if (CooldownTimer < 0)
        {
            UseSkill();
            CooldownTimer = cooldown;
            return true;
        }
        
        Debug.Log("技能冷却中");
        return false;
    }
    
    public virtual void UseSkill()
    {
        //一些特殊的技能
    }
}
