using UnityEngine;

public interface IProjectileEffect
{
    void ApplyEffect(EnemyScript enemy, BulletScript bullet);
}