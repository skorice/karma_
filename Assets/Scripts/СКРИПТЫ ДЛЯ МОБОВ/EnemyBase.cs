using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class EnemyBase : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected float maxHealth = 50f;
    [SerializeField] protected float moveSpeed = 3f;
    [SerializeField] protected float turnSpeed = 4f;

    [SerializeField] protected float damage = 5f;
    [SerializeField] protected float attackRadius = 1.2f;
    [SerializeField] protected float attackCooldown = 1f;

    [Header("Drop")]
    [SerializeField] protected GameObject karmaDropPrefab;
    [SerializeField] protected int karmaValue = 1;

    [Header("Stun")]
    [SerializeField] protected float stunDuration = 0.5f;
    
    [Header("Separation")]
    [SerializeField] protected float separationRadius = 1.1f;        // private -> protected
    [SerializeField] protected float separationStrength = 2.5f;      // private -> protected
    [SerializeField] private LayerMask enemyMask;

    protected float currentHealth;
    protected Transform player;
    private float attackTimer;
    public float Damage => damage;

    protected SpriteRenderer spriteRenderer;
    private Color originalColor;

    protected bool isStunned;
    private float stunTimer;

    protected Vector2 currentVelocity;

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

            currentVelocity = Vector2.zero;
            return;
        }

        if (player == null) return;

        Move();
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            TryAttack();
        }
    }

    protected virtual void Move()
    {
        Vector2 myPos = transform.position;
        Vector2 toPlayer = (Vector2)player.position - myPos;

        Vector2 desiredDirection = toPlayer.sqrMagnitude > 0.0001f
            ? toPlayer.normalized
            : Vector2.zero;

        Vector2 desiredVelocity = desiredDirection * moveSpeed;
        desiredVelocity += GetSeparation(myPos) * separationStrength;

        if (desiredVelocity.sqrMagnitude > moveSpeed * moveSpeed)
            desiredVelocity = desiredVelocity.normalized * moveSpeed;

        currentVelocity = Vector2.Lerp(
            currentVelocity,
            desiredVelocity,
            turnSpeed * Time.deltaTime
        );

        transform.position += (Vector3)(currentVelocity * Time.deltaTime);
    }

    // ИЗМЕНЕНО: private -> protected, чтобы наследники могли использовать
    protected Vector2 GetSeparation(Vector2 myPos)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            myPos,
            separationRadius,
            enemyMask
        );

        if (hits.Length <= 1)
            return Vector2.zero;

        Vector2 push = Vector2.zero;
        int count = 0;

        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject)
                continue;

            Vector2 away = myPos - (Vector2)hit.transform.position;
            float distSqr = away.sqrMagnitude;

            if (distSqr < 0.0001f)
            {
                away = Random.insideUnitCircle;
                if (away.sqrMagnitude < 0.0001f)
                    away = Vector2.right;
                distSqr = 0.01f;
            }

            float dist = Mathf.Sqrt(distSqr);
            push += away / dist;
            count++;
        }

        if (count == 0)
            return Vector2.zero;

        return push / count;
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

    protected void ApplyStun()
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}