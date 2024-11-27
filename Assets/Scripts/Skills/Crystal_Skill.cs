using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal_Skill : Skill
{
    [SerializeField] private float crystalDuration;
    [SerializeField] private GameObject crystalPrefab;
    private GameObject currentCrystal;
    
    [Header("Explosive crystal")]
    [SerializeField] private bool canExplode;
    
    [Header("Moving crystal")]
    [SerializeField] private bool canMoveToEnemy;
    [SerializeField] private float moveSpeed;

    [Header("Multi stacking crystal")] 
    [SerializeField] private bool canUseMultiStacks;
    [SerializeField] private int amountOfStacks;
    [SerializeField] private float multiStackCooldown;
    [SerializeField] private float useTimeWindow;
    [SerializeField] private List<GameObject> crystalLeft = new List<GameObject>();
    public override void UseSkill()
    {
        base.UseSkill();

        if(CanUseMultiCrystal())
            return;
        
        if (currentCrystal == null)
        {
            currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
            Crystal_Skill_Controller currentCrystalScript = currentCrystal.GetComponent<Crystal_Skill_Controller>();
            currentCrystalScript.SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindCloestEnemy(currentCrystal.transform));
        }
        else
        {
            if (canMoveToEnemy)
                return;
            
            Vector2 playerPos = player.transform.position;
            player.transform.position = currentCrystal.transform.position;
            currentCrystal.transform.position = playerPos;
            currentCrystal.GetComponent<Crystal_Skill_Controller>()?.FinishCrystal();
        }
    }

    private bool CanUseMultiCrystal()
    {
        if (canUseMultiStacks)
        {
            // 能够生成多个水晶时, 且水晶列表不为空时, 则生成一个水晶
            if (crystalLeft.Count > 0)
            {
                if(crystalLeft.Count == amountOfStacks)
                    Invoke(nameof(ResetAbility), useTimeWindow);
                cooldown = 0;
                GameObject crystalToSpawn = crystalLeft[crystalLeft.Count - 1];
                GameObject newCrystal = Instantiate(crystalToSpawn, player.transform.position, Quaternion.identity);
                crystalLeft.Remove(crystalToSpawn);
                newCrystal.GetComponent<Crystal_Skill_Controller>().SetupCrystal(
                    crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindCloestEnemy(newCrystal.transform));
                if(crystalLeft.Count <= 0)
                {
                    // 冷却技能并重置水晶列表
                    cooldown = multiStackCooldown;
                    RefileCrystal();
                }
                return true;
            }
        }
        return false;
    }
    private void RefileCrystal()
    {
        int amountToAdd = amountOfStacks - crystalLeft.Count;
        for (int i = 0; i < amountToAdd; i++)
            crystalLeft.Add(crystalPrefab);
    }

    private void ResetAbility()
    {
        if(cooldownTimer > 0)
            return;
        cooldownTimer -= multiStackCooldown;
        RefileCrystal();
    }
}
