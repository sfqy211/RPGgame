using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Blackhole_Skill_Controller : MonoBehaviour
{
    [SerializeField] private GameObject hotKeyPrefab;
    [SerializeField] private List<KeyCode> keyCodeList;
    
    private float maxSize;
    private float growSpeed;
    private float shrinkSpeed;
    private float blackHoleTimer;
    
    private bool canGrow = true;
    private bool canShrink;
    private bool canCreatHotKey = true;
    private bool cloneAttackReleased;
    private bool playerCanDisapear = true;
    
    private int amountOfAttacks = 4;
    private float cloneAttackCooldown = .3f;
    private float cloneAttackTimer;

    private List<Transform> targets = new List<Transform>();
    private List<GameObject> createdHotKey = new List<GameObject>();
    
    public bool playerCanExitState { get; private set; }

    public void SetupBlackhole(float _maxSize, float _growSpeed, float _shrinkSpeed, int _amountOfAttacks, float _cloneAttackCooldown, float _blackHoleDuration)
    {
        maxSize = _maxSize;
        growSpeed = _growSpeed;
        shrinkSpeed = _shrinkSpeed;
        amountOfAttacks = _amountOfAttacks;
        cloneAttackCooldown = _cloneAttackCooldown;
        blackHoleTimer = _blackHoleDuration;
    }
    
    private void Update()
    {
        cloneAttackTimer -= Time.deltaTime;
        blackHoleTimer -= Time.deltaTime;
        
        if (blackHoleTimer < 0)
        {
            blackHoleTimer = Mathf.Infinity;
            if (targets.Count > 0)
                ReleaseCloneAttack();
            else
                FinishBlackHoleAbility();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ReleaseCloneAttack();
        }

        CloneAttackLogic();
        
        if (canGrow && !canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);
        }

        if (canShrink)
        {
                transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-1, -1), shrinkSpeed * Time.deltaTime);
                if (transform.localScale.x < 0)
                    Destroy(gameObject);
        }
    }

    private void ReleaseCloneAttack()
    {
        if (targets.Count <= 0)
            return;
        
        DestoryHotKey();
        cloneAttackReleased = true;
        canCreatHotKey = false;
        if (playerCanDisapear)
        {
            playerCanDisapear = false;
            PlayerManager.instance.player.MakeTransparent(true);
        }
        
    }

    private void CloneAttackLogic()
    {
        if (cloneAttackTimer < 0 && cloneAttackReleased && amountOfAttacks > 0)
        {
            // 检查 targets 不为空且还有攻击次数
            // if (!(targets.Count > 0 && amountOfAttacks > 0))
            // {
            //     return true;
            // }
            cloneAttackTimer = cloneAttackCooldown;
            int randomIndex = Random.Range(0, targets.Count);

            float xOffset;

            if (Random.Range(0, 100) > 50)
                xOffset = 1;
            else
                xOffset = -1;
            
            SkillManager.instance.clone.CreatClone(targets[randomIndex], new Vector3(xOffset, 0));
            amountOfAttacks--;

            if (amountOfAttacks <= 0)
                Invoke("FinishBlackHoleAbility", 1.2f);
        }
    }

    private void FinishBlackHoleAbility()
    {
        DestoryHotKey();
        playerCanExitState = true;
        canShrink = true;
        cloneAttackReleased = false;
        // 此处注释的是退出黑洞技能的功能，已用其他方法实现
        // PlayerManager.instance.player.ExitBlackholeAbility();
    }

    private void DestoryHotKey()
    {
        if(createdHotKey.Count < 0)
                return;
        for (int i = 0; i < createdHotKey.Count; i++)
            Destroy(createdHotKey[i]);;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            collision.GetComponent<Enemy>().FreezeTime(true);

            CreatHotKey(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.GetComponent<Enemy>() != null)
            collision.GetComponent<Enemy>().FreezeTime(false);
    }

    private void CreatHotKey(Collider2D collision)
    {
        if (keyCodeList.Count <= 0)
        {
            Debug.LogWarning("没有足够热键");
            return;
        }
        
        if(!canCreatHotKey)
            return;
        
        GameObject newHotKey = Instantiate(hotKeyPrefab, collision.transform.position + new Vector3(0, 2),
            Quaternion.identity);
        createdHotKey.Add(newHotKey);
            
        KeyCode choosenKey = keyCodeList[Random.Range(0, keyCodeList.Count)];
            
        keyCodeList.Remove(choosenKey);
            
        Blackhole_HotKey_Controller newHotKeyScript = newHotKey.GetComponent<Blackhole_HotKey_Controller>();
            
        newHotKeyScript.SetupHotKey(choosenKey, collision.transform, this);
    }
    
    public void AddEnenyToList(Transform _enemyTransform) => targets.Add(_enemyTransform);
}
