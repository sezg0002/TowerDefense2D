using UnityEngine;

public class BulletScript : FlyingShotScript
{
    public IProjectileEffect ProjectileEffect; 
    public IBulletVisualDecorator BulletVisualDecorator;
    public Vector2 Direction;
    public float Speed;
    public float Range;
    public float Damage;
    public GameObject Target;
    public System.Collections.Generic.List<string> EnemyTags;
    public Transform Turret;

    private float distance;

    public CannonScript.EffectType EffectTypeTag;

    public void Init()
    {
        if (BulletVisualDecorator != null)
            BulletVisualDecorator.ApplyVisual(this);
        else
        {
            var sprite = GetComponent<SpriteRenderer>();
            if (sprite != null)
                sprite.color = Color.white;
        }
    }

    void OnEnable()
    {
        distance = 0.0f;
        Pool.Instance.ActivateObject("rifleSoundEffect").SetActive(true);
    }

    void FixedUpdate()
    {
        var diff = Time.deltaTime * Speed;
        distance += diff;
        transform.position += (Vector3)Direction * diff;

        if (distance > Range)
        {
            Pool.Instance.DeactivateObject(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<EnemyScript>();
            if (enemy != null && enemy.gameObject.activeInHierarchy)
            {
                if (ProjectileEffect != null)
                {
                    ProjectileEffect.ApplyEffect(enemy, this);
                }
                else
                {
                    enemy.TakeDamage(Damage);
                }
            }
            if (enemy != null)
            {
                EnemyScript.EffectColorType colorType = EnemyScript.EffectColorType.None;
                switch (EffectTypeTag)
                {
                    case CannonScript.EffectType.Fire: colorType = EnemyScript.EffectColorType.Fire; break;
                    case CannonScript.EffectType.Ice: colorType = EnemyScript.EffectColorType.Ice; break;
                    case CannonScript.EffectType.Wind: colorType = EnemyScript.EffectColorType.Wind; break;
                }
                enemy.SetColorByEffect(colorType);
            }
            Pool.Instance.DeactivateObject(gameObject);
        }
    }
}