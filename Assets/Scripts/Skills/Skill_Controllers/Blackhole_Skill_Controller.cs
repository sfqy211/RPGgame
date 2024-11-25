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
    
    private bool canGrow = true;
    private bool canShrink;
    private bool canCreantHotKey = true;
    private bool cloneAttackReleased;
    
    private int amountOfAttacks = 4;
    private float cloneAttackCooldown = .3f;
    private float cloneAttackTimer;

    private List<Transform> targets = new List<Transform>();
    private List<GameObject> createdHotKey = new List<GameObject>();

    public void SetupBlackhole(float _maxSize, float _growSpeed, float _shrinkSpeed, int _amountOfAttacks, float _cloneAttackCooldown)
    {
        maxSize = _maxSize;
        growSpeed = _growSpeed;
        shrinkSpeed = _shrinkSpeed;
        amountOfAttacks = _amountOfAttacks;
        cloneAttackCooldown = _cloneAttackCooldown;
    }
    
    private void Update()
    {
        cloneAttackTimer -= Time.deltaTime;

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
                {
                    Destroy(gameObject);
                }
        }
    }

    private void ReleaseCloneAttack()
    {
        DestoryHotKey();
        cloneAttackReleased = true;
        canCreantHotKey = false;
        PlayerManager.instance.player.MakeTransparent(true);
    }

    private void CloneAttackLogic()
    {
        if (cloneAttackTimer < 0 && cloneAttackReleased)
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
                xOffset = 2;
            else
                xOffset = -2;
            
            SkillManager.instance.clone.CreatClone(targets[randomIndex], new Vector3(xOffset, 0));
            amountOfAttacks--;

            if (amountOfAttacks <= 0)
            {
                Invoke("FinishBlackHoleAbility", 1.5f);
            }
        }
    }

    private void FinishBlackHoleAbility()
    {
        PlayerManager.instance.player.ExitBlackholeAbility();
        canShrink = true;
        cloneAttackReleased = false;
    }

    private void DestoryHotKey()
    {
        if(createdHotKey.Count < 0)
                return;
        for (int i = 0; i < createdHotKey.Count; i++)
        {
            Destroy(createdHotKey[i]);;
        }
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
        
        if(!canCreantHotKey)
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
