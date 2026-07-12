using UnityEngine;

public class AnomalyManager : MonoBehaviour
{
    [Header("Список префабов аномалий")]
    [SerializeField] private GameObject[] anomalyPrefabs;

    [Header("Точки спавна аномалий (3 точки)")]
    [SerializeField] private Transform[] spawnPoints;

    private GameObject spawnedAnomaly;


    private void Start()
    {
        GenerateAnomaly();
    }


    public void GenerateAnomaly()
    {
        // 50% шанс появления аномалии
        bool hasAnomaly = Random.value > 0.5f;

        // Передаём состояние в CaveState
        CaveState.Instance.SetAnomalyState(hasAnomaly);

        if (!hasAnomaly)
        {
            Debug.Log("Аномалия НЕ появилась в этой пещере");
            return;
        }

        // Выбираем случайный префаб
        int prefabIndex = Random.Range(0, anomalyPrefabs.Length);
        GameObject prefab = anomalyPrefabs[prefabIndex];

        // Выбираем случайную точку спавна
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        Transform point = spawnPoints[spawnIndex];

        // Спавним аномалию
        spawnedAnomaly = Instantiate(prefab, point.position, Quaternion.identity);

        Debug.Log($"Появилась аномалия: {prefab.name} в точке {spawnIndex + 1}");
    }
}
