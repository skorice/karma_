using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private EnemyBase[] enemyPrefabs;

    [Header("Spawn")]
    [SerializeField] private Transform player;
    [SerializeField] private float spawnRadius = 12f;
    [SerializeField] private float waveInterval = 25f;

    [Header("Wave")]
    [SerializeField] private int firstWaveCount = 15;
    [SerializeField] private float multiplier = 1.2f;

    private bool canSpawn;
    private int currentWaveCount;

    private void Start()
    {
        currentWaveCount = firstWaveCount;
    }

    public void StartSpawning()
    {
        canSpawn = true;
        StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        canSpawn = false;
        StopAllCoroutines();
    }

    IEnumerator SpawnLoop()
    {
        while (canSpawn)
        {
            SpawnWave();

            yield return new WaitForSeconds(waveInterval);

            currentWaveCount =
                Mathf.RoundToInt(currentWaveCount * multiplier);
        }
    }

    void SpawnWave()
    {
        for (int i = 0; i < currentWaveCount; i++)
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        if (player == null)
        return;
        
        Vector2 direction = Random.insideUnitCircle.normalized;

        Vector2 spawnPosition =
            (Vector2)player.position + direction * spawnRadius;

        int randomEnemy =
            Random.Range(0, enemyPrefabs.Length);

        Instantiate(
            enemyPrefabs[randomEnemy],
            spawnPosition,
            Quaternion.identity);
    }
}