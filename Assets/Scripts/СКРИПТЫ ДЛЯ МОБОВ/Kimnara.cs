using UnityEngine;

public class Kimnara : EnemyBase
{
    [Header("Range")]
    [SerializeField] private float minDistance = 4f;

    [SerializeField] private float maxDistance = 6f;

    [Header("Attack")]
    [SerializeField] private GameObject projectilePrefab;

    [SerializeField] private float shootCooldown = 3f;

    private float baseHealth = 30f;
    private float baseSpeed;
    private float baseDamage=10f;

    private float shootTimer;

    protected override void Update()
    {
        if (player == null)
        return;
        
        Move();

        shootTimer += Time.deltaTime;

        if (shootTimer >= shootCooldown)
        {
            Shoot();
            shootTimer = 0;
        }
    }

    protected override void Move()
    {
        float distance =
            Vector2.Distance(transform.position, player.position);

        if (distance > maxDistance)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                moveSpeed * Time.deltaTime);
        }
        else if (distance < minDistance)
        {
            Vector2 dir =
                (transform.position - player.position).normalized;

            transform.position +=
                (Vector3)(dir * moveSpeed * Time.deltaTime);
        }
    }

    private void Shoot()
    {
        if (projectilePrefab == null)
            return;

        Instantiate(
            projectilePrefab,
            transform.position,
            Quaternion.identity);
    }
}