using UnityEngine;

public class KarmaDrop : MonoBehaviour
{
    [SerializeField] private float destroyTime = 8.4f;
    [SerializeField] private float pickupDelay = 0.5f; // Задержка перед подбором
    [SerializeField] private int minValue = 1;
    [SerializeField] private int maxValue = 5;

    private int value;
    private float spawnTime;
    private bool canPickup;

    public void SetValue(int amount)
    {
        value = amount;
    }

    private void Start()
    {
        if (value == 0)
        {
            value = Random.Range(minValue, maxValue + 1);
        }

        spawnTime = Time.time;
        canPickup = false;

        Destroy(gameObject, destroyTime);
    }

    private void Update()
    {
        // Проверяем, прошла ли задержка
        if (!canPickup && Time.time - spawnTime >= pickupDelay)
        {
            canPickup = true;
            Debug.Log($"⏳ Карма готова к подбору! (задержка {pickupDelay}с)");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Если задержка не прошла — игнорируем
        if (!canPickup)
        {
            Debug.Log($"⏳ Карма ещё не готова! Подождите {pickupDelay - (Time.time - spawnTime):F1}с");
            return;
        }

        PlayerLevelManager levelManager = other.GetComponent<PlayerLevelManager>();

        if (levelManager != null)
        {
            levelManager.AddKarma(value);
            Debug.Log($"💎 Игрок подобрал {value} кармы! Всего: {levelManager.GetCurrentKarma()}, Уровень: {levelManager.GetCurrentLevel()}");
        }
        else
        {
            Debug.LogWarning("⚠️ У игрока нет компонента PlayerLevelManager!");
        }

        Destroy(gameObject);
    }
}