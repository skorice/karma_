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
        if (!other.CompareTag("Player"))
            return;

        // ИНТЕГРАЦИЯ С КОДОМ ИГРОКА 
        //
        // По ТЗ:
        // - скорость игрока -20%
        // - сила атаки -10%
        //
        // Здесь необходимо вызвать метод системы игрока,
        // который накладывает дебафф.
        // PlayerSettings settings = other.GetComponent<PlayerSettings>();
        // settings.ApplySnakeDebuff();
    

        Debug.Log("Player entered Snake Trail");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;


        // ИНТЕГРАЦИЯ С КОДОМ ИГРОКА 
        // Здесь необходимо убрать дебафф и вернуть характеристики игрока.
        // PlayerSettings settings = other.GetComponent<PlayerSettings>();
        // settings.RemoveSnakeDebuff();
    

        Debug.Log("Player exited Snake Trail");
    }
}