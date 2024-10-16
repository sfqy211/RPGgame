using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCounterAttackState : PlayerState
{
    public PlayerCounterAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        stateTimer = player.counterAttackDuration;
        player.anim.SetBool("SuccessfulCounterAttack", false);
    }


    public override void Exit()
    {
        base.Exit();
    }
    public override void Update()
    {
        base.Update();
        
        player.SetZeroVelocity();
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);
        
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                if (hit.GetComponent<Enemy>().CanBeStunned())
                {
                    stateTimer = 10;//任何值都可以，只要大于1
                    player.anim.SetBool("SuccessfulCounterAttack", true);
                }
            }
        }
        
        if(stateTimer < 0 || triggeredCalled)
            stateMachine.ChangeState(player.idleState);
    }
}
