using UnityEngine;

namespace EnemyState
{
    public class FrozenState : IEnemyState
    {
        private float duration = 1f;
        private float timer;
        private float slowFactor = 0.6f;
        private IEnemyState previousState;

        public FrozenState(IEnemyState previousState = null)
        {
            this.previousState = previousState;
        }

        public void Enter(EnemyScript enemy)
        {
            timer = duration;
            enemy.SetColorByEffect(EnemyScript.EffectColorType.Ice);
            enemy.SetSpeedFactor(slowFactor);
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