public class IceEffect : IProjectileEffect
{
    public void ApplyEffect(EnemyScript enemy, BulletScript bullet)
    {
        enemy.TakeDamage(bullet.Damage); 
        enemy.ApplySlow(1.5f, 0.5f);     
    }
}