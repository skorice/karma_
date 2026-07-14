using UnityEngine;
using System.Collections;

public class Kimnara : EnemyBase
{
    [Header("Range")]
    [SerializeField] private float minDistance = 4f;
    [SerializeField] private float maxDistance = 6f;

    [Header("Attack")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float shootCooldown = 3f;

    private float shootTimer;
    private Vector2 targetDirection;

    protected override IEnumerator Start()
    {
        yield return base.Start();
        shootTimer = 0f;
    }

    protected override void Update()
    {
        base.Update();
        
        if (player == null || isStunned)
            return;

        shootTimer += Time.deltaTime;
        if (shootTimer >= shootCooldown)
        {
            Shoot();
            shootTimer = 0;
        }
    }

    protected override void Move()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > maxDistance)
        {
            targetDirection = (player.position - transform.position).normalized;
        }
        else if (distance < minDistance)
        {
            targetDirection = (transform.position - player.position).normalized;
        }
        else
        {
            targetDirection = Vector2.zero;
        }

        // Используем базовую логику движения с separation
        Vector2 myPos = transform.position;
        Vector2 desiredVelocity = targetDirection * moveSpeed;

        // Теперь GetSeparation и separationStrength доступны! ✅
        desiredVelocity += GetSeparation(myPos) * separationStrength;

        if (desiredVelocity.sqrMagnitude > moveSpeed * moveSpeed)
            desiredVelocity = desiredVelocity.normalized * moveSpeed;

        // Плавное изменение скорости
        currentVelocity = Vector2.Lerp(
            currentVelocity,
            desiredVelocity,
            turnSpeed * Time.deltaTime
        );

        transform.position += (Vector3)(currentVelocity * Time.deltaTime);
    }

    private void Shoot()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("ProjectilePrefab не назначен в Kimnara!");
            return;
        }

        // Создаем снаряд
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        
        // Получаем компонент Projectile
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null && player != null)
        {
            // Направляем снаряд к игроку
            Vector2 direction = (player.position - transform.position).normalized;
            projectileScript.SetDirection(direction);
            Debug.Log($"{name} выпустил снаряд в сторону игрока!");
        }
        else
        {
            Debug.LogWarning("Projectile component not found on prefab!");
        }
    }
}