using System.Collections;
using UnityEngine;

public class Player : Entity
{
    [Header("Attack details 攻击参数")]
    public Vector2[] attackMovement;
    public float counterAttackDuration = .2f;
    
    public bool isBusy { get; private set; }
    [Header("Move info 移动信息")]
    public float moveSpeed = 12f;
    public float jumpForce;
    public float swordReturnImpact;
    private float defaultMoveSpeed;
    private float defaultJumpForce;
    
    [Header("Dash info 冲刺信息")]
    public float dashSpeed;
    public float dashDuration;
    private float defaultDashSpeed;
    public float dashDir{ get; private set; }
    
    
    public SkillManager skill { get; private set; }
    public GameObject sword { get; private set; }

    
    #region States

    public PlayerStateMachine stateMachine { get; private set;}
    public PlayerIdleState idleState { get; private set;}
    public PlayerMoveState moveState { get; private set;}
    public  PlayerJumpState jumpState { get; private set;}
    public PlayerAirState airState { get; private set;}
    public PlayerWallSlideState wallSlide { get; private set;}
    public PlayerWallJumpState wallJump { get; private set;}
    public PlayerDashState dashState { get; private set;}
    public PlayerPrimaryAttackState primaryAttackState { get; private set;}
    public PlayerCounterAttackState counterAttackState { get; private set;}
    public PlayerAimSwordState aimSword { get; private  set;}
    public  PlayerCatchSwordState catchSword { get; private  set;}
    public PlayerBlackholeState blackHole { get; private set; }
    public PlayerDeadState deadState { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();
        stateMachine = new PlayerStateMachine();
        
        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState  = new PlayerAirState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlide = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJump  = new PlayerWallJumpState(this, stateMachine, "Jump");
        
        primaryAttackState = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        counterAttackState = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");
        
        aimSword   = new PlayerAimSwordState(this, stateMachine, "AimSword");
        catchSword = new PlayerCatchSwordState(this, stateMachine, "CatchSword");
        blackHole  = new PlayerBlackholeState(this, stateMachine, "Jump");
        deadState  = new PlayerDeadState(this, stateMachine, "Die");
    }
    
    protected override void Start()
    {
        base.Start();
        
        skill = SkillManager.instance;
        
        stateMachine.Initialize(idleState);
        
        defaultMoveSpeed = moveSpeed;
        defaultJumpForce = jumpForce;
        defaultDashSpeed = dashSpeed;
    }
    
    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
        
        CheckForDashInput();
        if (Input.GetKeyDown(KeyCode.F))
            skill.crystal.CanUseSkill();
    }

    public override void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {
        moveSpeed *= (1 - _slowPercentage);
        jumpForce *= (1 - _slowPercentage);
        dashSpeed *= (1 - _slowPercentage);
        anim.speed *= (1 - _slowPercentage);
        Invoke(nameof(ReturnDefaultSpeed), _slowDuration);
    }

    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();
        moveSpeed = defaultMoveSpeed;
        jumpForce = defaultJumpForce;
        dashSpeed = defaultDashSpeed;
        
    }

    public void AssignNewSword(GameObject _newSword)
    {
        sword = _newSword;
    }
    public void CatchTheSword()
    {
        stateMachine.ChangeState(catchSword);
        Destroy(sword);
    }
    public IEnumerator BusyFor(float _seconds)
    {
        isBusy = true;
        
        yield return  new WaitForSeconds(_seconds);
        
        isBusy = false;
    }
    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();

    private void CheckForDashInput() 
    {
        if (IsWallDetected())
            return;
        
        if (Input.GetKeyDown(KeyCode.LeftShift) && SkillManager.instance.dash.CanUseSkill())
        {
            dashDir = Input.GetAxisRaw("Horizontal");

            if (dashDir == 0)
                dashDir = facingDir;
            
            stateMachine.ChangeState(dashState);
        }
    }

    public override void Die()
    {
        base.Die();
        
        stateMachine.ChangeState(deadState);
    }
}
