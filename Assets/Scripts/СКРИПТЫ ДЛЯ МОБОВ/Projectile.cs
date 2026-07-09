using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float damage = 6f;
    [SerializeField] private float lifeTime = 5f;

    private Transform player;
    private Vector2 direction;

    private void Start()
    {
        Destroy(gameObject, lifeTime);

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
            direction = (player.position - transform.position).normalized;
        }
    }

    private void Update()
{
    if (player == null)
    {
        Destroy(gameObject);
        return;
    }

    transform.position +=
        (Vector3)(direction * speed * Time.deltaTime);
}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        // ==========================================================
        // ЗАГЛУШКА ДЛЯ ТЕСТИРОВАНИЯ
        // ВРЕМЕННО используется PlayerHealth.cs.
        // После интеграции заменить на систему от Юли.
        // ==========================================================

        PlayerSettings health = other.GetComponent<PlayerSettings>();

        if (health != null)
        {
            health.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}