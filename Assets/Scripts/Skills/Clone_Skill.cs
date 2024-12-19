using System.Collections;
using UnityEngine;

public class Clone_Skill : Skill
{
    [Header("Clone Info 克隆信息")]
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;
    [Space]
    [SerializeField] private bool canAttack;
    
    [SerializeField] private bool creatCloneOnDashStart;
    [SerializeField] private bool creatCloneOnDashOver;
    [SerializeField] private bool canCreateCloneOnCounterAttack;
    [Header("Clone can duplicate 克隆可以复制")]
    [SerializeField] private bool canDuplicateClone;
    [SerializeField] private float chanceToDuplicate;
    [Header("Crystal instead of clone 用水晶代替克隆")]
    public bool crystalInsteadOfClone;
    public void CreatClone(Transform _clonePosition, Vector3 _offset)
    {
        if (crystalInsteadOfClone)
        {
            SkillManager.instance.crystal.CreateCrystal();
            return;
        }
        GameObject newClone = Instantiate(clonePrefab);
        
        newClone.GetComponent<Clone_Skill_Controller>().
            SetupClone(_clonePosition, cloneDuration, canAttack, _offset, FindClosestEnemy(newClone.transform), canDuplicateClone, chanceToDuplicate, player);
    }

    public void CreatCloneOnDashStart()
    {
        if(creatCloneOnDashStart)
            CreatClone(player.transform, Vector3.zero);
    }

    public void CreatCloneOnDashOver()
    {
        if(creatCloneOnDashOver)
            CreatClone(player.transform, Vector3.zero);
    }

    public void CreatCloneOnCounterAttack(Transform _enemyTransform)
    {
        if (canCreateCloneOnCounterAttack)
            StartCoroutine(CreatCloneWithDelay(_enemyTransform, new Vector3(2 * player.facingDir, 0)));
    }

    private IEnumerator CreatCloneWithDelay(Transform _transform, Vector3 _offset)
    {
        yield return new WaitForSeconds(0.4f);
            CreatClone(_transform, _offset);
    }
}
