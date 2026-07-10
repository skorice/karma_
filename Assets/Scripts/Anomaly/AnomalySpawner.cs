using UnityEngine;

public class AnomalySpawner : MonoBehaviour
{
    [Header("Базовая пещера")]
    public GameObject baseCavePrefab;

    [Header("Префабы аномалий")]
    public GameObject[] anomalyPrefabs;

    [Header("Настройки")]
    [Range(0, 1)]
    public float anomalyChance = 0.5f;

    private bool currentHasAnomaly = false;
    private GameObject currentCave;
    private GameObject currentAnomaly;
    private int lastAnomalyIndex = -1; // ЗАПОМИНАЕМ ПОСЛЕДНЮЮ АНОМАЛИЮ

    public void GenerateNewLocation()
    {
        // Удаляем старую аномалию
        if (currentAnomaly != null)
        {
            Destroy(currentAnomaly);
            currentAnomaly = null;
        }

        // Удаляем старую пещеру
        if (currentCave != null)
        {
            Destroy(currentCave);
            currentCave = null;
        }

        // Проверяем, есть ли префаб пещеры
        if (baseCavePrefab == null)
        {
            Debug.LogError("Нет префаба базовой пещеры!");
            return;
        }

        // Спавним новую пещеру
        currentCave = Instantiate(baseCavePrefab, transform.position, Quaternion.identity);

        // Решаем, будет ли аномалия
        currentHasAnomaly = Random.value < anomalyChance;

        // Находим точку для спавна аномалии
        Transform spawnPoint = currentCave.transform.Find("AnomalySpawnPoint");
        if (spawnPoint == null)
        {
            spawnPoint = currentCave.transform;
        }

        if (currentHasAnomaly && anomalyPrefabs.Length > 0)
        {
            int randomIndex;
            
            // ВЫБИРАЕМ АНОМАЛИЮ, КОТОРАЯ НЕ БЫЛА ПРОШЛОЙ
            do
            {
                randomIndex = Random.Range(0, anomalyPrefabs.Length);
            } 
            while (randomIndex == lastAnomalyIndex && anomalyPrefabs.Length > 1);
            
            lastAnomalyIndex = randomIndex; // ← ЗАПОМИНАЕМ
            
            GameObject selectedAnomaly = anomalyPrefabs[randomIndex];

            if (selectedAnomaly != null)
            {
                currentAnomaly = Instantiate(selectedAnomaly, spawnPoint.position, Quaternion.identity);
                currentAnomaly.transform.SetParent(spawnPoint);
                Debug.Log($"СПАВНЕНО: {selectedAnomaly.name}");
            }
        }
        else
        {
            Debug.Log($"АНОМАЛИЙ НЕТ (шанс {anomalyChance * 100}%)");
        }

        // Обновляем информацию в пещере
        CaveInfo caveInfo = currentCave.GetComponent<CaveInfo>();
        if (caveInfo != null)
        {
            caveInfo.hasAnomaly = currentHasAnomaly;
            caveInfo.caveName = currentHasAnomaly ? "Пещера с аномалией" : "Обычная пещера";
        }

        Debug.Log($"ПЕЩЕРА ЗАГРУЖЕНА | Аномалия: {(currentHasAnomaly ? "✅ ЕСТЬ" : "❌ НЕТ")}");
    }

    public bool HasAnomaly() => currentHasAnomaly;
}