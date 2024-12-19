using UnityEngine;

public class PlayerBlackholeState : PlayerState
{
    private readonly float flyTime = .4f;
    private bool skillUsed;
    private float defaultGravity;

    public PlayerBlackholeState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        defaultGravity = player.rb.gravityScale;
        
        skillUsed = false;
        stateTimer = flyTime;
        rb.gravityScale = 0;
    }

    public override void Exit()
    {
        base.Exit();
        player.rb.gravityScale = defaultGravity;
        player.fx.MakeTransparent(false);
    }
    
    public override void Update()
    {
        base.Update();

        if (stateTimer > 0)
            rb.velocity = new Vector2(0, 15);
        if (stateTimer < 0)
        {
            rb.velocity = new Vector2(0, -.1f);

            if (!skillUsed)
            {
                Debug.Log("释放黑洞");
                if (player.skill.blackHole.CanUseSkill())
                {
                    skillUsed = true;
                }
            }
            // 当一个黑洞技能的所有攻击结束后退出该状态
            if (player.skill.blackHole.SkillCompleted())
            {
                stateMachine.ChangeState(player.airState);
            }
        }
        
    }
    
    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }
}
