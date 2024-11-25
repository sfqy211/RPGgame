using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    [SerializeField] private float cooldown;
    protected float cooldownTimer;

    protected Player player;
    
    protected virtual void Start()
    {
        player = PlayerManager.instance.player;
    }
    
    protected virtual void Update()
    {
        cooldownTimer -= Time.deltaTime;
    }

    public virtual bool CanUseSkill()
    {
        if (cooldownTimer < 0)
        {
            UseSkill();
            cooldownTimer = cooldown;
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
