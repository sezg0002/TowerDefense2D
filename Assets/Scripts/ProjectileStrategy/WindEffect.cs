public class WindEffect : IProjectileEffect
{
    public void ApplyEffect(EnemyScript enemy, BulletScript bullet)
    {
        enemy.TakeDamage(bullet.Damage); 
        enemy.ApplyKnockback(bullet.Direction, 10.0f);
    }
}
