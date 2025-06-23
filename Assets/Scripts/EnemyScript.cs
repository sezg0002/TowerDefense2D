using Assets.Scripts;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using EnemyState; 

public class EnemyScript : MonoBehaviour
{
    public float MaxHealth;
    public int Money;
    public GameObject Coin;
    public float SpawnedCoinMean;
    public float SpawnedCoinStd;

    private Transform canvas;
    private Slider healthBar;
    private float health;

    private IEnemyState currentState;
    public IEnemyState CurrentState
    {
        get { return currentState; }
    }

    private Coroutine slowCoroutine;
    private float baseSpeed; 

    private Coroutine burnCoroutine;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine colorEffectCoroutine;

    private void Awake()
    {
        PathFollower pathFollower = GetComponent<PathFollower>();
        if (pathFollower != null)
            baseSpeed = pathFollower.Speed;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    private void OnEnable()
    {
        canvas = transform.Find("Canvas");
        healthBar = canvas.Find("HealthBar").GetComponent<Slider>();
        canvas.gameObject.SetActive(false);

        health = MaxHealth;
        healthBar.maxValue = MaxHealth;
        healthBar.value = health;

        PathFollower pathFollower = GetComponent<PathFollower>();
        if (pathFollower != null)
            pathFollower.Speed = baseSpeed;

        if (slowCoroutine != null)
        {
            StopCoroutine(slowCoroutine);
            slowCoroutine = null;
        }
        if (burnCoroutine != null)
        {
            StopCoroutine(burnCoroutine);
            burnCoroutine = null;
        }
        if (colorEffectCoroutine != null)
        {
            StopCoroutine(colorEffectCoroutine);
            colorEffectCoroutine = null;
        }
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        ChangeState(new NormalState());
    }

    private void Update()
    {
        canvas.rotation = Quaternion.identity;
        canvas.localScale = Vector3.one * 0.5f;
        if (currentState != null)
            currentState.Update(this);
    }

    private void SpawnCoins()
    {
        var num = (int)(MathHelpers.NextGaussianDouble() * SpawnedCoinStd + SpawnedCoinMean + 0.5f);

        for (int i = 0; i < num; i++)
        {
            var x = MathHelpers.NextGaussianDouble() * Mathf.Log(i + 1) * 4.0f;
            var y = MathHelpers.NextGaussianDouble() * Mathf.Log(i + 1) * 4.0f;

            var coin = Pool.Instance.ActivateObject(Coin.tag);
            coin.transform.position = transform.position + new Vector3(x, y, 0);
            coin.SetActive(true);
        }

        GameManager.Instance.AddMoney(Money);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!gameObject.activeSelf) return;

        if (collision.CompareTag("finish"))
        {
            GameManager.Instance.EnemyEscaped(gameObject);
        }
        else if ((collision.CompareTag("bullet") && !CompareTag("plane")) || (collision.CompareTag("rocket") && !CompareTag("soldier")))
        {
            var flyingShot = collision.gameObject.GetComponent<FlyingShotScript>();
            var damage = flyingShot.Damage;

            if (currentState != null)
                currentState.OnHit(this, damage);
            else
                TakeDamageInternal(damage);

            canvas.gameObject.SetActive(true);
            flyingShot.BlowUp();

            if (IsDead())
            {
                if (CompareTag("plane") || CompareTag("tank"))
                {
                    Pool.Instance.ActivateObject("bigExplosionSoundEffect").SetActive(true);
                    var explosion = Pool.Instance.ActivateObject("explosionParticle");
                    explosion.transform.position = transform.position;
                    explosion.SetActive(true);
                }

                SpawnCoins();
                GameManager.Instance.EnemyKilled(gameObject);
                Pool.Instance.DeactivateObject(gameObject);
                EnemyManagerScript.Instance.DeleteEnemy(gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "finish")
        {
            EnemyManagerScript.Instance.DeleteEnemy(gameObject);
            Pool.Instance.DeactivateObject(gameObject);
        }
    }

    public void TakeDamage(float amount)
    {
        if (currentState != null)
            currentState.OnHit(this, amount);
        else
            TakeDamageInternal(amount);
    }

    public void TakeDamageInternal(float amount)
    {
        health -= amount;
        healthBar.value = health;
        canvas.gameObject.SetActive(true);

        if (IsDead())
        {
            SpawnCoins();
            GameManager.Instance.EnemyKilled(gameObject);
            Pool.Instance.DeactivateObject(gameObject);
            EnemyManagerScript.Instance.DeleteEnemy(gameObject);
        }
    }

    public bool IsDead()
    {
        return health <= 0;
    }

    public void ChangeState(IEnemyState newState)
    {
        if (currentState != null)
            currentState.Exit(this);
        currentState = newState;
        if (currentState != null)
            currentState.Enter(this);
    }


    public void SetSpeedFactor(float factor)
    {
        PathFollower pf = GetComponent<PathFollower>();
        if (pf != null)
            pf.Speed = baseSpeed * factor;
    }

    public void SetSpeedToBase()
    {
        PathFollower pf = GetComponent<PathFollower>();
        if (pf != null)
            pf.Speed = baseSpeed;
    }

    public void ResetColor()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }

    public enum EffectColorType { Fire, Ice, Wind, None }

    public void SetColorByEffect(EffectColorType effect)
    {
        if (spriteRenderer == null || !gameObject.activeInHierarchy)
            return;

        if (colorEffectCoroutine != null)
        {
            StopCoroutine(colorEffectCoroutine);
            colorEffectCoroutine = null;
        }

        Color effectColor = originalColor;
        switch (effect)
        {
            case EffectColorType.Fire:
                effectColor = Color.red;
                break;
            case EffectColorType.Ice:
                effectColor = Color.blue;
                break;
            case EffectColorType.Wind:
                effectColor = Color.green;
                break;
            default:
                effectColor = originalColor;
                break;
        }
        spriteRenderer.color = effectColor;

        colorEffectCoroutine = StartCoroutine(ResetColorAfterDelay(0.5f));
    }

    private IEnumerator ResetColorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
        colorEffectCoroutine = null;
    }

    public void ApplyKnockback(Vector2 from, float force)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null) return;

        Vector2 direction = ((Vector2)transform.position - from).normalized;
        rb.AddForce(direction * force, ForceMode2D.Impulse);

        PathFollower pf = GetComponent<PathFollower>();
        if (pf != null)
            pf.PauseForKnockback(0.4f); 
    }

    public void ApplyBurn(float duration, float damagePerSecond)
    {
        ChangeState(new BurnedState(damagePerSecond, currentState));
    }

    public void ApplySlow(float duration, float slowFactor)
    {
        ChangeState(new FrozenState(currentState));
    }

    public void ApplyWind(float duration, float speedMultiplier)
    {
        ChangeState(new WindState(currentState));
    }
}