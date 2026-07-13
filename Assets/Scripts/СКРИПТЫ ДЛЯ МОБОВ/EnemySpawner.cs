using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyTypeInfo
    {
        public EnemyBase prefab;
        public int waveUnlock;
        [Range(0f, 100f)]
        public float quotaPercent;
        public bool isResidual;
    }

    [Header("Enemy Types Configuration")]
    [SerializeField] private List<EnemyTypeInfo> enemyTypes;

    [Header("Spawn")]
    [SerializeField] private Transform player;

    [Header("Map Bounds")]
    [SerializeField] private float halfWidth = 800f;
    [SerializeField] private float halfHeight = 430f;
    [SerializeField] private float mapPadding = 20f;

    [Header("Random Spawn Settings")]
    [SerializeField] private float minimumDistanceFromPlayer = 120f;
    [SerializeField] private float minimumDistanceBetweenSpawns = 5f;
    [SerializeField] private int positionSearchAttempts = 30;

    private bool canSpawn;

    private readonly List<Vector2> currentWaveSpawnPositions = new List<Vector2>();

    private IEnumerator Start()
    {
        while (GameObject.FindGameObjectWithTag("Player") == null)
            yield return null;

        player = GameObject.FindGameObjectWithTag("Player").transform;
        Debug.Log("EnemySpawner: игрок найден, спавн готов.");
    }

    public void StartSpawning() => canSpawn = true;
    public void StopSpawning() => canSpawn = false;

    public void SpawnWaveWithTypes(int waveIndex, int waveSize)
    {
        if (!canSpawn)
            return;

        if (player == null)
        {
            Debug.LogWarning("EnemySpawner: игрок ещё не появился, волна не будет заспавнена.");
            return;
        }

        if (enemyTypes == null || enemyTypes.Count == 0)
        {
            Debug.LogError("Список мобов для спавна не заполнен");
            return;
        }

        currentWaveSpawnPositions.Clear();

        Dictionary<EnemyTypeInfo, int> counts = new Dictionary<EnemyTypeInfo, int>();
        int totalOther = 0;

        foreach (var type in enemyTypes)
        {
            if (type.isResidual)
                continue;

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

        EnemyTypeInfo residualType = enemyTypes.Find(t => t.isResidual);

        if (residualType != null)
        {
            int residualCount = Mathf.Max(0, waveSize - totalOther);
            counts[residualType] = residualCount;
        }

        foreach (var kvp in counts)
        {
            int count = kvp.Value;

            if (count <= 0)
                continue;

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
            Debug.LogWarning("EnemySpawner: игрок ещё не появился — враг не будет заспавнен.");
            return;
        }

        if (enemyPrefab == null)
            return;

        if (!TryFindRandomSpawnPosition(out Vector2 spawnPosition))
        {
            Debug.LogWarning("Не удалось найти безопасную позицию для спавна врага.");
            return;
        }

        currentWaveSpawnPositions.Add(spawnPosition);

        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }

    private bool TryFindRandomSpawnPosition(out Vector2 spawnPosition)
    {
        float minX = -halfWidth + mapPadding;
        float maxX = halfWidth - mapPadding;
        float minY = -halfHeight + mapPadding;
        float maxY = halfHeight - mapPadding;

        spawnPosition = Vector2.zero;

        if (minX > maxX || minY > maxY)
        {
            Debug.LogError("Некорректные размеры области спавна.");
            return false;
        }

        Vector2 playerPosition = player.position;

        float safeDistanceSqr = minimumDistanceFromPlayer * minimumDistanceFromPlayer;
        float spawnDistanceSqr = minimumDistanceBetweenSpawns * minimumDistanceBetweenSpawns;

        int attempts = Mathf.Max(1, positionSearchAttempts);

        for (int attempt = 0; attempt < attempts; attempt++)
        {
            Vector2 candidate = new Vector2(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY)
            );

            if ((candidate - playerPosition).sqrMagnitude < safeDistanceSqr)
                continue;

            if (IsTooCloseToAnotherSpawn(candidate, spawnDistanceSqr))
                continue;

            spawnPosition = candidate;
            return true;
        }

        return false;
    }

    private bool IsTooCloseToAnotherSpawn(Vector2 position, float minimumDistanceSqr)
    {
        foreach (Vector2 otherPosition in currentWaveSpawnPositions)
        {
            if ((position - otherPosition).sqrMagnitude < minimumDistanceSqr)
                return true;
        }

        return false;
    }
}
