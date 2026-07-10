using UnityEngine;

public class KarmaDrop : MonoBehaviour
{
    [SerializeField] private float destroyTime = 8.4f;
    [SerializeField] private int minValue = 1;
    [SerializeField] private int maxValue = 5;

    private int value;

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

        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;


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