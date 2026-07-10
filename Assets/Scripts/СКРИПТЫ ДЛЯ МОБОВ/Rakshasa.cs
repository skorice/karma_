using UnityEngine;

public class Rakshasa : EnemyBase
{
    [Header("Ranged Attack")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float shootCooldown = 3f;
    [SerializeField] private float minDistance = 5f;
    [SerializeField] private float maxDistance = 8f;

    [Header("Pack Buff")]
    [SerializeField] private float checkRadius = 3f;
    [SerializeField] private int requiredAllies = 4;

    private float shootTimer;
    private float baseSpeed;
    private float baseDamage;
    private float buffTimer = 0f; // ← добавлено!

    protected override void Start()
    {
        base.Start();
        baseSpeed = moveSpeed;
        baseDamage = damage;
    }

    protected override void Update()
    {
        base.Update();

        // Перемещение с учётом дистанции
        float distance = 0f; // ← объявляем ЗДЕСЬ, чтобы была доступна во всём методе
        if (player != null)
        {
            distance = Vector2.Distance(transform.position, player.position);

            if (distance > maxDistance)
            {
                // Приближаемся
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    player.position,
                    moveSpeed * Time.deltaTime);
            }
            else if (distance < minDistance)
            {
                // Отступаем
                Vector2 dir = (transform.position - player.position).normalized;
                transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);
            }
            // иначе стоим на месте
        }

        // Стрельба
        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0 && player != null && distance <= maxDistance)
        {
            Shoot();
            shootTimer = shootCooldown;
        }

        // Обновление баффа стаи
        buffTimer += Time.deltaTime;
        if (buffTimer >= 0.5f)
        {
            UpdatePackBonus();
            buffTimer = 0f;
        }
    }

    private void Shoot()
    {
        if (projectilePrefab == null) return;
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile projectile = proj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.SetDirection(direction);
        }
    }

    private void UpdatePackBonus()
    {
        Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, checkRadius);
        int count = 0;
        foreach (Collider2D col in nearby)
        {
            if (col.GetComponent<Rakshasa>() != null)
                count++;
        }

        if (count >= requiredAllies)
        {
            moveSpeed = baseSpeed * 1.1f;
            damage = baseDamage * 1.15f;
        }
        else
        {
            moveSpeed = baseSpeed;
            damage = baseDamage;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
#endif
}