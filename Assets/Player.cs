using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Move info")]
    public  float moveSpeed = 12f;
    public   float jumpForce;

    [Header("Collision info")]
    [SerializeField] Transform groundCheck;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private  LayerMask whatIsGround;
    
    public int facingDir { get; private set; } = 1;
    private bool facingRight = true;
    
    #region Components

    public Animator anim { get; private  set;}
    public Rigidbody2D rb { get; private set;}

    #endregion

    #region States

    public PlayerStateMachine stateMachine { get; private set;}
    
    public PlayerIdleState idleState { get; private set;}
    public PlayerMoveState moveState { get; private set;}
    public  PlayerJumpState jumpState { get; private set;}
    public PlayerAirState airState { get; private set;}

    #endregion

    private void Awake()
    {
        stateMachine = new PlayerStateMachine();
        
        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState = new PlayerAirState(this, stateMachine, "Jump");
    }
    
    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        
        stateMachine.Initialize(idleState);
    }
    
    private void Update()
    {
        stateMachine.currentState.Update();
        
    }

    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        
        rb.velocity = new Vector2(_xVelocity, _yVelocity);
        FlipController(_xVelocity);
    }
    
    public bool IsGroundDetected()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
    }
    
    public  void Flip()
    {
        facingDir = -facingDir;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    public void FlipController(float _x)
    {
        if (_x > 0 && !facingRight)
        {
            Flip();
        }
        else if (_x < 0 && facingRight)
        {
            Flip();
        }
    }
}
