using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Scripts;

public class CannonScript : MonoBehaviour
{
    public GameObject BulletPrototype;
    public List<GameObject> Enemies;

    public List<TowerUpgradeData> upgrades;
    public int currentLevel = 0;

    private float ShootingPeriod;
    private float Range;
    private float BulletSpeed;
    private float Damage;
    private float RotationSpeed;

    public float RangeValue { get { return Range; } }
    public float DamageValue { get { return Damage; } }
    public float ShootingPeriodValue { get { return ShootingPeriod; } }
    public float BulletSpeedValue { get { return BulletSpeed; } }
    public float RotationSpeedValue { get { return RotationSpeed; } }

    public enum EffectType { Default, Fire, Ice, Wind }
    public EffectType SelectedType = EffectType.Default;
    private IProjectileEffect currentEffect;

    private GameObject bulletPlaceholder;
    private float timeToShoot = 0.0f;
    private List<string> enemyTags;

    void Start()
    {
        ApplyUpgrade(currentLevel); 
        enemyTags = Enemies.Select(e => e.tag).ToList();

        var enemy = EnemyManagerScript.Instance.GetEnemyInRange(transform.position, float.PositiveInfinity, enemyTags);
        if (enemy != null)
        {
            var angle = MathHelpers.Angle(enemy.transform.position - transform.position, transform.up);
            transform.eulerAngles = new Vector3(0, 0, angle);
        }
        var rocketTransform = transform.Find("Rocket");
        bulletPlaceholder = rocketTransform != null ? rocketTransform.gameObject : null;
    }

    void FixedUpdate()
    {
        var enemy = EnemyManagerScript.Instance.GetEnemyInRange(transform.position, Range, enemyTags);

        if (enemy != null)
        {
            TurnToEnemy(enemy.transform.position + enemy.transform.right * 32);

            if (timeToShoot < 0)
            {
                var bullet = Pool.Instance.ActivateObject(BulletPrototype.tag);

                bullet.transform.position = transform.position;
                bullet.transform.rotation = transform.rotation;

                var bulletScript = bullet.GetComponent<BulletScript>();
                bulletScript.Speed = BulletSpeed;
                bulletScript.Range = Range;
                bulletScript.Direction = transform.up;
                bulletScript.Damage = Damage;
                bulletScript.Target = enemy;
                bulletScript.EnemyTags = enemyTags;
                bulletScript.Turret = transform;

                bulletScript.ProjectileEffect = currentEffect;

                if (SelectedType == EffectType.Fire)
                {
                    bulletScript.BulletVisualDecorator = new FireBulletVisualDecorator();
                }
                else if (SelectedType == EffectType.Ice)
                {
                    bulletScript.BulletVisualDecorator = new IceBulletVisualDecorator();
                }
                else if (SelectedType == EffectType.Wind)
                {
                    bulletScript.BulletVisualDecorator = new WindBulletVisualDecorator();
                }
                else
                {
                    bulletScript.BulletVisualDecorator = null;
                }

                bulletScript.EffectTypeTag = SelectedType;

                bulletScript.Init();

                bullet.SetActive(true);

                timeToShoot = ShootingPeriod;

                if (bulletPlaceholder != null) bulletPlaceholder.SetActive(false);
                return;
            }
        }
        else
        {
            var closestEnemy = EnemyManagerScript.Instance.GetClosestEnemyInRange(transform.position, Range * 2, enemyTags);
            if (closestEnemy != null) TurnToEnemy(closestEnemy.transform.position + closestEnemy.transform.right * 32);
        }

        if (bulletPlaceholder != null && timeToShoot < ShootingPeriod / 8.0f && !bulletPlaceholder.activeSelf)
        {
            bulletPlaceholder.SetActive(true);
        }

        timeToShoot -= Time.deltaTime;
    }

    private void TurnToEnemy(Vector2 position)
    {
        var direction = position - (Vector2)transform.position;
        var angle = MathHelpers.Angle(direction, transform.up);
        transform.Rotate(0, 0, Mathf.Clamp(angle, -10, 10) * RotationSpeed * Time.deltaTime);
    }

    public void SetEffectStrategy(EffectType type)
    {
        SelectedType = type;
        switch (SelectedType)
        {
            case EffectType.Fire:
                currentEffect = new FireEffect();
                break;
            case EffectType.Ice:
                currentEffect = new IceEffect();
                break;
            case EffectType.Wind:
                currentEffect = new WindEffect();
                break;
            case EffectType.Default:
            default:
                currentEffect = null; 
                break;
        }
    
        var visuals = GetComponentInParent<TowerVisuals>();
        if (visuals != null)
        {
            visuals.SetVisuals(ConvertToTowerType(SelectedType));
        }
        
        Debug.Log("Effet défini : " + SelectedType);
    }

    public void SetTowerType(TowerFactory.TowerType type)
    {
        EffectType effectType = ConvertToEffectType(type);
        SetEffectStrategy(effectType);
    }

    private EffectType ConvertToEffectType(TowerFactory.TowerType type)
    {
        switch(type)
        {
            case TowerFactory.TowerType.Fire:
                return EffectType.Fire;
            case TowerFactory.TowerType.Ice:
                return EffectType.Ice;
            case TowerFactory.TowerType.Wind:
                return EffectType.Wind;
            case TowerFactory.TowerType.Basic:
            default:
                return EffectType.Default; 
        }
    }

    private global::TowerType ConvertToTowerType(EffectType effectType)
    {
        switch(effectType)
        {
            case EffectType.Fire:
                return global::TowerType.Fire;
            case EffectType.Ice:
                return global::TowerType.Ice;
            case EffectType.Wind:
                return global::TowerType.Wind;
            default:
                return global::TowerType.Basic;
        }
    }

    public bool CanUpgrade()
    {
        return currentLevel < upgrades.Count - 1;
    }

    public int GetNextUpgradeCost()
    {
        if (CanUpgrade()) return upgrades[currentLevel + 1].upgradeCost;
        return -1; 
    }

    public void Upgrade()
    {
        if (CanUpgrade())
        {
            currentLevel++;
            ApplyUpgrade(currentLevel);
            Debug.Log("Tourelle améliorée au niveau " + currentLevel);
        }
    }

    private void ApplyUpgrade(int level)
    {
        if (level >= 0 && level < upgrades.Count)
        {
            var data = upgrades[level];
            ShootingPeriod = data.shootingPeriod;
            Range = data.range;
            BulletSpeed = data.bulletSpeed;
            Damage = data.damage;
            RotationSpeed = data.rotationSpeed;
            Debug.Log("Stats appliquées - Level: " + level + ", Range: " + Range + ", Damage: " + Damage);
        }
    }
}