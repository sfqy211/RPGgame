using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    [Header("Move info")] 
    public float moveSpeed;
    public float idleTime;
    public EnemyStateMachine StateMachine{ get; private set; }

    protected  override void Awake()
    {
        base.Awake();
        StateMachine = new EnemyStateMachine();
    }

    protected override void Update()
    {
        base.Update();
        StateMachine.currentState.Update();
    }
}   
