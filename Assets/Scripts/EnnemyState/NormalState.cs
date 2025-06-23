namespace EnemyState
{
    public class NormalState : IEnemyState
    {
        public void Enter(EnemyScript enemy)
        {
            enemy.ResetColor();
            enemy.SetSpeedToBase();
        }

        public void Update(EnemyScript enemy) { }

        public void Exit(EnemyScript enemy) { }

        public void OnHit(EnemyScript enemy, float damage)
        {
            enemy.TakeDamageInternal(damage);
        }
    }
}