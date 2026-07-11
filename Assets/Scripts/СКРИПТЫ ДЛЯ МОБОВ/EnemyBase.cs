using UnityEngine;

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

    protected float currentHealth;
    protected Transform player;

    private float attackTimer;
    public float Damage => damage;

    protected virtual void Start()
    {
        currentHealth = maxHealth;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    protected virtual void Update()
    {
        if (player == null)
            return;

        Move();

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            TryAttack();
        }
    }

    protected virtual void Move()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            moveSpeed * Time.deltaTime);
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
            else
            {
                Debug.LogWarning($"{name}: PlayerHealth не найден на игроке!");
            }

            attackTimer = attackCooldown;
        }
    }

    public virtual void TakeDamage(float amount)
    {
        if (currentHealth <= 0)
            return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Debug.Log($"💀 {name} умер! Попытка спавна кармы...");

        if (karmaDropPrefab != null)
        {
            Debug.Log($"✅ Префаб кармы есть: {karmaDropPrefab.name}");
            GameObject drop = Instantiate(karmaDropPrefab, transform.position, Quaternion.identity);
            KarmaDrop karma = drop.GetComponent<KarmaDrop>();
            if (karma != null)
            {
                karma.SetValue(karmaValue);
                Debug.Log($"💎 Спавнена карма значением {karmaValue}");
            }
            else
            {
                Debug.LogWarning($"⚠️ На префабе {karmaDropPrefab.name} нет компонента KarmaDrop!");
            }
        }
        else
        {
            Debug.LogWarning($"⚠️ У {name} не назначен KarmaDropPrefab!");
        }

        Destroy(gameObject);
    }
}