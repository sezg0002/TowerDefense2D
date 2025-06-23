public class FireEffect : IProjectileEffect
{
    public void ApplyEffect(EnemyScript enemy, BulletScript bullet)
    {
        enemy.TakeDamage(bullet.Damage);  
        enemy.ApplyBurn(2.0f, 0.4f);     
    }
}