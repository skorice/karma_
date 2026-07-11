using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class EnemyBase : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected float maxHealth = 50f;
    [SerializeField] protected float moveSpeed = 3f;
    [SerializeField] protected float damage = 5f;
    [SerializeField] protected float attackRadius = 1.2f;
    [SerializeField] protected float attackCooldown = 1f;

    [Header("Drop")]
    [SerializeField] protected GameObject karmaDropPrefab;
    [SerializeField] protected int karmaValue = 1;

    [Header("Stun")]
    [SerializeField] protected float stunDuration = 0.5f; // длительность заморозки

    protected float currentHealth;
    protected Transform player;
    private float attackTimer;
    public float Damage => damage;

    protected SpriteRenderer spriteRenderer;
    private Color originalColor;

    // Для стауна
    private bool isStunned;
    private float stunTimer;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;
    }

    protected virtual void Update()
    {
        // Обновляем стаун
        if (isStunned)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0)
            {
                isStunned = false;
                // Можно добавить лог для отладки
                // Debug.Log($"{name} вышел из стауна");
            }
            // Если в стауне — не двигаемся и не атакуем
            return;
        }

        if (player == null) return;

        Move();
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
            TryAttack();
    }

    protected virtual void Move()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            moveSpeed * Time.deltaTime
        );
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

        // Визуальный эффект (красная вспышка)
        StartCoroutine(FlashRed());

        // Стаун (заморозка)
        ApplyStun();

        if (currentHealth <= 0)
            Die();
    }

    private void ApplyStun()
    {
        isStunned = true;
        stunTimer = stunDuration;
    }

    private IEnumerator FlashRed()
    {
        if (spriteRenderer == null) yield break;
        spriteRenderer.color = new Color(1f, 0.2f, 0.2f, 0.7f);
        yield return new WaitForSeconds(0.15f);
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