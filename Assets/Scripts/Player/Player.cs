using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : Entity
{
    [Header("Attack details")]
    public Vector2[] attackMovement;
    public float counterAttackDuration = .2f;
    
    public bool isBusy { get; private set; }
    [Header("Move info")]
    public  float moveSpeed = 12f;
    public   float jumpForce;
    
    [Header("Dash info")]
    public float dashSpeed;
    public float dashDuration;
    public float dashDir{ get; private set; }
    
    
    public SkillManager skill { get; private set; }

    
    #region States

    public PlayerStateMachine stateMachine { get; private set;}
    
    public PlayerIdleState idleState { get; private set;}
    public PlayerMoveState moveState { get; private set;}
    public  PlayerJumpState jumpState { get; private set;}
    public PlayerAirState airState { get; private set;}
    public PlayerWallSlideState wallSlide { get; private set;}
    public PlayerWallJumpState wallJump { get; private set;}
    public PlayerDashState dashState { get; private set;}
    
    public PlayerPrimaryAttackState PrimaryAttackState { get; private set;}
    public PlayerCounterAttackState counterAttackState { get; private set;}
    #endregion

    protected override void Awake()
    {
        base.Awake();
        stateMachine = new PlayerStateMachine();
        
        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState = new PlayerAirState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlide = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJump = new PlayerWallJumpState(this, stateMachine, "Jump");
        
        PrimaryAttackState = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        counterAttackState = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");
    }
    
    protected override void Start()
    {
        base.Start();
        
        skill = SkillManager.instance;
        
        stateMachine.Initialize(idleState);
    }
    
    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
        
        CheckForDashInput();
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
}
