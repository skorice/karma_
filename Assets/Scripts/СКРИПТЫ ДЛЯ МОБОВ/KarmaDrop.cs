using UnityEngine;

public class KarmaDrop : MonoBehaviour
{
    [SerializeField] private float destroyTime = 8.4f;
    [SerializeField] private int minValue = 1;    // Минимальное значение
    [SerializeField] private int maxValue = 5;    // Максимальное значение

    private int value;

    public void SetValue(int amount)
    {
        value = amount;
    }

    private void Start()
    {
        // Если значение не установлено, генерируем случайное
        if (value == 0)
        {
            value = Random.Range(minValue, maxValue + 1);
        }

        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Находим компонент PlayerLevel на игроке
        PlayerLevel playerLevel = other.GetComponent<PlayerLevel>();

        if (playerLevel != null)
        {
            playerLevel.AddKarma(value);
            Debug.Log($"💎 Игрок подобрал {value} кармы! Всего: {playerLevel.CurrentKarma}, Уровень: {playerLevel.CurrentLevel}");
        }
        else
        {
            Debug.LogWarning("⚠️ У игрока нет компонента PlayerLevel!");
        }

        Destroy(gameObject);
    }
}