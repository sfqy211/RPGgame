using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Skeleton : Enemy
{

    #region States

    public SkeletonIdleState idleState{ get; private set; }
    public SkeletonMoveState moveState{ get; private set; }
    public  SkeletonBattleState battleState{ get; private set; }
    public SkeletonAttackState attackState{ get; private set; }
    public SkeletonStunnedState stunnedState{ get; private set; }
    public SkeletonDeadState deadState{ get; private set; }

    #endregion
    protected override void Awake()
    {
        base.Awake();

        idleState = new SkeletonIdleState(this, StateMachine, "Idle", this);
        moveState = new SkeletonMoveState(this, StateMachine, "Move", this);
        battleState = new SkeletonBattleState(this, StateMachine, "Move", this);
        attackState = new SkeletonAttackState(this, StateMachine, "Attack", this);
        stunnedState = new SkeletonStunnedState(this, StateMachine, "Stunned", this);
        deadState = new SkeletonDeadState(this, StateMachine, "Idle", this);
    }

    protected override void Start()
    {
        base.Start();
        StateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();
    }

    public override bool CanBeStunned()
    {
        if (base.CanBeStunned())
        {
            StateMachine.ChangeState(stunnedState);
            return true;
        }
        return false;
    }

    public override void Die()
    {
        base.Die();
        StateMachine.ChangeState(deadState);
    }
}
