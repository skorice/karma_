using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyTypeInfo
    {
        public EnemyBase prefab;
        public int waveUnlock; // с какой волны доступен

        [Range(0f, 100f)]
        public float quotaPercent; // % от общего числа врагов в волне

        public bool isResidual; // получает остаток врагов
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
    [Tooltip("Минимальное расстояние от игрока до точки появления врага")]
    [SerializeField] private float minimumDistanceFromPlayer = 120f;

    [Tooltip("Минимальное расстояние между врагами, создаваемыми в одной волне")]
    [SerializeField] private float minimumDistanceBetweenSpawns = 5f;

    [Tooltip("Количество попыток найти подходящую позицию")]
    [SerializeField] private int positionSearchAttempts = 30;

    private bool canSpawn;

    // Позиции врагов, уже созданных во время текущего вызова спавна волны.
    private readonly List<Vector2> currentWaveSpawnPositions = new List<Vector2>();

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
        if (!canSpawn)
            return;

        if (enemyTypes == null || enemyTypes.Count == 0)
        {
            Debug.LogError("Список мобов для спавна не заполнен");
            return;
        }

        // Очищаем только позиции текущей волны.
        currentWaveSpawnPositions.Clear();

        // Считаем, сколько врагов каждого типа нужно спавнить.
        Dictionary<EnemyTypeInfo, int> counts =
            new Dictionary<EnemyTypeInfo, int>();

        int totalOther = 0;

        foreach (var type in enemyTypes)
        {
            if (type.isResidual)
                continue;

            // Проверяем, доступен ли тип на этой волне.
            if (waveIndex + 1 >= type.waveUnlock)
            {
                int count = Mathf.FloorToInt(
                    waveSize * type.quotaPercent / 100f
                );

                counts[type] = count;
                totalOther += count;
            }
            else
            {
                counts[type] = 0;
            }
        }

        // Ищем тип, который получает оставшихся врагов.
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

        // Спавн.
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
            Debug.LogError("Player not assigned in EnemySpawner!");
            return;
        }

        if (enemyPrefab == null)
            return;

        if (!TryFindRandomSpawnPosition(out Vector2 spawnPosition))
        {
            Debug.LogWarning(
                "Не удалось найти безопасную позицию для спавна врага. " +
                "Проверьте размеры карты и Minimum Distance From Player."
            );
            return;
        }

        currentWaveSpawnPositions.Add(spawnPosition);

        Instantiate(
            enemyPrefab,
            spawnPosition,
            Quaternion.identity
        );
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
            Debug.LogError(
                "Некорректные размеры области спавна. " +
                "Map Padding превышает размеры карты."
            );

            return false;
        }

        Vector2 playerPosition = player.position;

        float safeDistance = Mathf.Max(0f, minimumDistanceFromPlayer);
        float safeDistanceSqr = safeDistance * safeDistance;

        float spawnDistance = Mathf.Max(0f, minimumDistanceBetweenSpawns);
        float spawnDistanceSqr = spawnDistance * spawnDistance;

        int attempts = Mathf.Max(1, positionSearchAttempts);

        for (int attempt = 0; attempt < attempts; attempt++)
        {
            Vector2 candidate = new Vector2(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY)
            );

            // Жёсткая безопасная зона вокруг игрока.
            // Точка внутри неё никогда не будет использована.
            if ((candidate - playerPosition).sqrMagnitude < safeDistanceSqr)
                continue;

            if (IsTooCloseToAnotherSpawn(candidate, spawnDistanceSqr))
                continue;

            spawnPosition = candidate;
            return true;
        }

        return false;
    }

    private bool IsTooCloseToAnotherSpawn(
        Vector2 position,
        float minimumDistanceSqr)
    {
        if (minimumDistanceSqr <= 0f)
            return false;

        foreach (Vector2 otherPosition in currentWaveSpawnPositions)
        {
            if ((position - otherPosition).sqrMagnitude <
                minimumDistanceSqr)
            {
                return true;
            }
        }

        return false;
    }
}