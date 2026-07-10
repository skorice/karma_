using UnityEngine;

public class GateTrigger : MonoBehaviour
{
    public enum GateSide { Left, Right }
    public GateSide side;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, что это игрок
        if (!other.CompareTag("Player")) return;

        // Отключаем все триггеры (чтобы не сработал второй)
        GateTrigger[] gates = FindObjectsOfType<GateTrigger>();
        foreach (var gate in gates)
        {
            gate.GetComponent<Collider2D>().enabled = false;
        }

        // Получаем информацию о пещере
        AnomalySpawner spawner = FindObjectOfType<AnomalySpawner>();
        if (spawner == null)
        {
            Debug.LogError("❌ AnomalySpawner не найден!");
            return;
        }

        bool hasAnomaly = spawner.HasAnomaly();
        bool choseLeft = (side == GateSide.Left);

        Debug.Log($"🚪 Игрок выбрал {(choseLeft ? "ЛЕВЫЙ" : "ПРАВЫЙ")} путь. Аномалия: {(hasAnomaly ? "ЕСТЬ" : "НЕТ")}");

        // Отправляем результат в CaveManager
        CaveManager.Instance.CheckPlayerChoice(hasAnomaly, choseLeft);
    }
}