using System.Collections.Generic;
using UnityEngine;
// using Random = UnityEngine.Random;

public class Blackhole_Skill_Controller : MonoBehaviour
{
    [SerializeField] private GameObject hotKeyPrefab;
    [SerializeField] private List<KeyCode> keyCodeList;
    
    private float maxSize;
    private float growSpeed;
    private float shrinkSpeed;
    private float blackHoleTimer;
    
    private readonly bool canGrow = true;
    private bool canShrink;
    private bool canCreatHotKey = true;
    private bool cloneAttackReleased;
    private bool playerCanDisappear = true;
    
    private int amountOfAttacks = 4;
    private float cloneAttackCooldown = .3f;
    private float cloneAttackTimer;

    private readonly List<Transform> targets = new List<Transform>();
    private readonly List<GameObject> createdHotKey = new List<GameObject>();
    
    public bool playerCanExitState { get; private set; }

    public void SetupBlackhole(float _maxSize, float _growSpeed, float _shrinkSpeed, int _amountOfAttacks, float _cloneAttackCooldown, float _blackHoleDuration)
    {
        maxSize = _maxSize;
        growSpeed = _growSpeed;
        shrinkSpeed = _shrinkSpeed;
        amountOfAttacks = _amountOfAttacks;
        cloneAttackCooldown = _cloneAttackCooldown;
        blackHoleTimer = _blackHoleDuration;
        if(SkillManager.instance.clone.crystalInsteadOfClone)
            playerCanDisappear = false;
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
            ReleaseCloneAttack();

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
        
        DestroyHotKey();
        cloneAttackReleased = true;
        canCreatHotKey = false;
        if (playerCanDisappear)
        {
            playerCanDisappear = false;
            PlayerManager.instance.player.fx.MakeTransparent(true);
        }
        
    }

    private void CloneAttackLogic()
    {
        if (cloneAttackTimer < 0 && cloneAttackReleased && amountOfAttacks > 0)
        {
            // 检查 targets 不为空且还有攻击次数
            // if (!(targets.Count > 0 && amountOfAttacks > 0))
            //     return true;
            cloneAttackTimer = cloneAttackCooldown;
            int randomIndex = Random.Range(0, targets.Count);

            float xOffset;

            if (Random.Range(0, 100) > 50)
                xOffset = 2;
            else
                xOffset = -2;
            if(SkillManager.instance.clone.crystalInsteadOfClone)
            {
                SkillManager.instance.crystal.CreateCrystal();
                SkillManager.instance.crystal.CurrentCrystalChooseRandomTarget();
            }
            else
            {
                SkillManager.instance.clone.CreatClone(targets[randomIndex], new Vector3(xOffset, 0));
            }

            SkillManager.instance.clone.CreatClone(targets[randomIndex], new Vector3(xOffset, 0));
            amountOfAttacks--;

            if (amountOfAttacks <= 0)
                Invoke(nameof(FinishBlackHoleAbility), 1f);
        }
    }

    private void FinishBlackHoleAbility()
    {
        DestroyHotKey();
        playerCanExitState = true;
        canShrink = true;
        cloneAttackReleased = false;
        // 此处注释的是退出黑洞技能的功能，已用其他方法实现
        // PlayerManager.instance.player.ExitBlackholeAbility();
    }

    private void DestroyHotKey()
    {
        if(createdHotKey.Count < 0)
            return;
        foreach (var t in createdHotKey)
            Destroy(t);
    }
    
    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if (_collision.GetComponent<Enemy>() != null)
        {
            _collision.GetComponent<Enemy>().FreezeTime(true);

            CreatHotKey(_collision);
        }
    }

    private void OnTriggerExit2D(Collider2D _collision)
    {
        if(_collision.GetComponent<Enemy>() != null)
            _collision.GetComponent<Enemy>().FreezeTime(false);
    }

    private void CreatHotKey(Collider2D _collision)
    {
        if (keyCodeList.Count <= 0)
        {
            Debug.LogWarning("没有足够热键");
            return;
        }
        
        if(!canCreatHotKey)
            return;
        
        GameObject newHotKey = Instantiate(hotKeyPrefab, _collision.transform.position + new Vector3(0, 2),
            Quaternion.identity);
        createdHotKey.Add(newHotKey);
            
        KeyCode choseKey = keyCodeList[Random.Range(0, keyCodeList.Count)];
            
        keyCodeList.Remove(choseKey);
            
        Blackhole_HotKey_Controller newHotKeyScript = newHotKey.GetComponent<Blackhole_HotKey_Controller>();
            
        newHotKeyScript.SetupHotKey(choseKey, _collision.transform, this);
    }
    
    public void AddEnemyToList(Transform _enemyTransform) => targets.Add(_enemyTransform);
}
