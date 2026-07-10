using UnityEngine;

public class SnakeTrail : MonoBehaviour
{
    [SerializeField] private float lifeTime = 8f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerSettings settings = other.GetComponent<PlayerSettings>();
        if (settings != null)
        {
            settings.ApplySnakeDebuff();
            Debug.Log("Player entered Snake Trail — debuff applied");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerSettings settings = other.GetComponent<PlayerSettings>();
        if (settings != null)
        {
            settings.RemoveSnakeDebuff();
            Debug.Log("Player exited Snake Trail — debuff removed");
        }
    }
}