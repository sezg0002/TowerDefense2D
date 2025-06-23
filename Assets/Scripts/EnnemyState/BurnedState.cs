using UnityEngine;

namespace EnemyState
{
    public class BurnedState : IEnemyState
    {
        private float duration = 2f;
        private float tick = 0.4f;
        private float timer;
        private float tickTimer;
        private float burnDamage;
        private IEnemyState previousState;

        public BurnedState(float burnDamage, IEnemyState previousState = null)
        {
            this.burnDamage = burnDamage;
            this.previousState = previousState;
        }

        public void Enter(EnemyScript enemy)
        {
            timer = duration;
            tickTimer = 0;
            enemy.SetColorByEffect(EnemyScript.EffectColorType.Fire);
        }

        public void Update(EnemyScript enemy)
        {
            timer -= Time.deltaTime;
            tickTimer -= Time.deltaTime;

            if (tickTimer <= 0f)
            {
                enemy.TakeDamageInternal(burnDamage);
                tickTimer = tick;
            }

            if (timer <= 0f)
            {
                enemy.ChangeState(previousState ?? new NormalState());
            }
        }

        public void Exit(EnemyScript enemy)
        {
            enemy.ResetColor();
        }

        public void OnHit(EnemyScript enemy, float damage)
        {
            enemy.TakeDamageInternal(damage);
        }
    }
}