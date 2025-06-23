using UnityEngine;

public class Tower : MonoBehaviour
{
    public int Damage = 10;
    public float Range = 5.0f;
    public float FireRate = 1.0f;
    public GameObject BulletPrefab;
    public IProjectileEffect ProjectileEffect; 

    private float fireCooldown = 0f;

    void Update()
    {
        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0)
        {
            var target = FindTarget();
            if (target != null)
            {
                FireAt(target);
                fireCooldown = 1f / FireRate;
            }
        }
    }

    void FireAt(GameObject target)
    {
        var bulletGO = Instantiate(BulletPrefab, transform.position, Quaternion.identity);
        var bullet = bulletGO.GetComponent<BulletScript>();
        bullet.Damage = Damage;
        bullet.Target = target;
        bullet.ProjectileEffect = ProjectileEffect;
    }

    GameObject FindTarget()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
        {
            if ((enemy.transform.position - transform.position).sqrMagnitude < Range * Range)
                return enemy;
        }
        return null;
    }
}