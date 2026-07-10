using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float damage = 6f;
    [SerializeField] private float lifeTime = 5f;

    private Vector2 direction = Vector2.zero;
    private bool isReady = false;

    private void Start()
    {
        Destroy(gameObject, lifeTime);

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            Vector2 dir = (playerObj.transform.position - transform.position).normalized;
            SetDirection(dir);
            Debug.Log($"Projectile created! Direction: {dir}");
        }
        else
        {
            Debug.LogWarning("Projectile: Player not found!");
            Destroy(gameObject);
        }
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        isReady = true;
        Debug.Log($"Projectile direction set to: {direction}");
    }

    private void Update()
    {
        if (!isReady || direction == Vector2.zero)
        {
            Debug.LogWarning($"Projectile: not ready! isReady={isReady}, direction={direction}");
            return;
        }

        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(damage);
            Debug.Log($"💥 Projectile hit player for {damage} damage!");
        }

        Destroy(gameObject);
    }
}