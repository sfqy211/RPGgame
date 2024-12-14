
public class PlayerStats : CharacterStats
{
    private Player player;
    
    protected override void Start()
    {
        base.Start();
        player = GetComponent<Player>();
    }

    protected override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
        player.DamageEffect();
    }

    protected override void Die()
    {
        base.Die();
        player.Die();
    }
}
