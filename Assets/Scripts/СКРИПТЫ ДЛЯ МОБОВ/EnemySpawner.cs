using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyTypeInfo
    {
        public EnemyBase prefab;  
        public int waveUnlock;          // с какой волны доступен
        [Range(0f, 100f)] public float quotaPercent; // % от общего числа врагов в волне
        public bool isResidual;         // true для типа, который получает остаток (Ракшаса)
    }

    [Header("Enemy Types Configuration")]
    [SerializeField] private List<EnemyTypeInfo> enemyTypes;

    [Header("Spawn")]
    [SerializeField] private Transform player;

    private bool canSpawn;
    private void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
                Debug.LogError("Игрок не найден");
        }
    }
    public void StartSpawning() => canSpawn = true;
    public void StopSpawning() => canSpawn = false;

    // Вызывается из BattleTimer для спавна волны
    public void SpawnWaveWithTypes(int waveIndex, int waveSize)
    {
        if (!canSpawn) return;
        if (enemyTypes == null || enemyTypes.Count == 0)
        {
            Debug.LogError("Список мобов для спавна не заполнен");
            return;
        }

        // Считаем, сколько врагов каждого типа нужно спавнить
        Dictionary<EnemyTypeInfo, int> counts = new Dictionary<EnemyTypeInfo, int>();
        int totalOther = 0;

        foreach (var type in enemyTypes)
        {
            if (type.isResidual) continue; // пропускаем, обработаем позже

            // Проверяем, доступен ли тип на этой волне
            if (waveIndex + 1 >= type.waveUnlock)
            {
                int count = Mathf.FloorToInt(waveSize * type.quotaPercent / 100f);
                counts[type] = count;
                totalOther += count;
            }
            else
            {
                counts[type] = 0;
            }
        }

        // IsResidual
        EnemyTypeInfo residualType = null;
        foreach (var type in enemyTypes)
        {
            if (type.isResidual)
            {
                residualType = type;
                break;
            }
        }

        if (residualType != null)
        {
            int residualCount = Mathf.Max(0, waveSize - totalOther);
            counts[residualType] = residualCount;
        }

        // Спавн
        foreach (var kvp in counts)
        {
            int count = kvp.Value;
            if (count <= 0) continue;

            for (int i = 0; i < count; i++)
            {
                SpawnEnemy(kvp.Key.prefab);
            }
        }
    }

    private void SpawnEnemy(EnemyBase enemyPrefab)
    {
        if (player == null)
        {
            Debug.LogError("Player not assigned in EnemySpawner!");
            return;
        }
        if (enemyPrefab == null) return;

        // Размеры карты
        float halfWidth = 800f;
        float halfHeight = 430f;
        float padding = 20f; // отступ от края, чтобы мобы не появлялись на границе

        // 4 угловые точки
        Vector2[] corners = new Vector2[]
        {
        new Vector2(-halfWidth + padding, -halfHeight + padding),
        new Vector2( halfWidth - padding, -halfHeight + padding),
        new Vector2(-halfWidth + padding,  halfHeight - padding),
        new Vector2( halfWidth - padding,  halfHeight - padding)
        };

        // Выбираем случайный угол
        Vector2 spawnPos = corners[Random.Range(0, corners.Length)];

        spawnPos += Random.insideUnitCircle * 10f;

        // Ограничиваем, чтобы не выйти за границы
        spawnPos.x = Mathf.Clamp(spawnPos.x, -halfWidth + padding, halfWidth - padding);
        spawnPos.y = Mathf.Clamp(spawnPos.y, -halfHeight + padding, halfHeight - padding);

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }
}
