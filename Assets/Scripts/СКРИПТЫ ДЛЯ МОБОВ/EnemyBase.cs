using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class EnemyBase : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected float maxHealth = 50f;
    [SerializeField] protected float moveSpeed = 3f;
    [SerializeField] private float turnSpeed = 4f;

    [SerializeField] protected float damage = 5f;
    [SerializeField] protected float attackRadius = 1.2f;
    [SerializeField] protected float attackCooldown = 1f;

    [Header("Drop")]
    [SerializeField] protected GameObject karmaDropPrefab;
    [SerializeField] protected int karmaValue = 1;

    [Header("Stun")]
    [SerializeField] protected float stunDuration = 0.5f;

    protected float currentHealth;
    protected Transform player;
    private float attackTimer;
    public float Damage => damage;

    protected SpriteRenderer spriteRenderer;
    private Color originalColor;

    private bool isStunned;
    private float stunTimer;

    private Vector2 currentVelocity;

    protected virtual IEnumerator Start()
    {
        currentHealth = maxHealth;

        while (GameObject.FindGameObjectWithTag("Player") == null)
            yield return null;

        player = GameObject.FindGameObjectWithTag("Player").transform;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;
    }

    protected virtual void Update()
    {
        if (isStunned)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0) isStunned = false;
            return;
        }

        if (player == null) return;

        Move();
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0) TryAttack();
    }

    protected virtual void Move()
    {
        Vector2 desiredDirection = ((Vector2)player.position - (Vector2)transform.position).normalized;
        Vector2 desiredVelocity = desiredDirection * moveSpeed;

        currentVelocity = Vector2.Lerp(currentVelocity, desiredVelocity, turnSpeed * Time.deltaTime);

        transform.position += (Vector3)(currentVelocity * Time.deltaTime);
    }

    protected virtual void TryAttack()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= attackRadius)
        {
            PlayerHealth health = player.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
                Debug.Log($"{name} атаковал игрока! Урон: {damage}");
            }
            attackTimer = attackCooldown;
        }
    }

    public virtual void TakeDamage(float amount)
    {
        if (currentHealth <= 0) return;
        currentHealth -= amount;

        StartCoroutine(FlashRed());
        ApplyStun();

        if (currentHealth <= 0) Die();
    }

    private void ApplyStun()
    {
        isStunned = true;
        stunTimer = stunDuration;
    }

    private IEnumerator FlashRed()
    {
        if (spriteRenderer == null) yield break;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.25f);
        spriteRenderer.color = originalColor;
    }

    protected virtual void Die()
    {
        Debug.Log($"💀 {name} умер! Попытка спавна кармы...");
        if (karmaDropPrefab != null)
        {
            GameObject drop = Instantiate(karmaDropPrefab, transform.position, Quaternion.identity);
            KarmaDrop karma = drop.GetComponent<KarmaDrop>();
            if (karma != null) karma.SetValue(karmaValue);
        }
        Destroy(gameObject);
    }
}
