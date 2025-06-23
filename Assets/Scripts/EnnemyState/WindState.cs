using UnityEngine;

namespace EnemyState
{
    public class WindState : IEnemyState
    {
        private float duration = 0.7f;
        private float timer;
        private float speedMultiplier = 1.7f;
        private IEnemyState previousState;

        public WindState(IEnemyState previousState = null)
        {
            this.previousState = previousState;
        }

        public void Enter(EnemyScript enemy)
        {
            timer = duration;
            enemy.SetColorByEffect(EnemyScript.EffectColorType.Wind);
            enemy.SetSpeedFactor(speedMultiplier);
        }

        public void Update(EnemyScript enemy)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                enemy.SetSpeedToBase();
                enemy.ChangeState(previousState ?? new NormalState());
            }
        }

        public void Exit(EnemyScript enemy)
        {
            enemy.ResetColor();
            enemy.SetSpeedToBase();
        }

        public void OnHit(EnemyScript enemy, float damage)
        {
            enemy.TakeDamageInternal(damage);
        }
    }
}