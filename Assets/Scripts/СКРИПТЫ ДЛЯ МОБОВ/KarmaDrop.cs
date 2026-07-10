using UnityEngine;

public class KarmaDrop : MonoBehaviour
{
    [SerializeField] private float destroyTime = 8.4f;

    private int value;

    public void SetValue(int amount)
    {
        value = amount;
    }

    private void Start()
    {
        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        
        // ИНТЕГРАЦИЯ С СИСТЕМОЙ ИГРОКА 
        //
        // Подобранная карма должна увеличить опыт/уровень игрока.
        //
        // Здесь надо вызвать метод системы опыта игрока:PlayerLevel level = other.GetComponent<PlayerLevel>();
        // level.AddKarma(value);

        Debug.Log($"Player picked up {value} karma.");

        Destroy(gameObject);
    }
}