using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleTimer : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] private float battleDuration = 90f;

    [Header("UI")]
    [SerializeField] private TMP_Text timerText;

    [Header("Spawn")]
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private float waveInterval = 25f;

    [Header("References")]
    [SerializeField] private PlayerSettings playerSettings;
    [SerializeField] private PlayerHealth playerHealth;

    [Header("TimeSlider")]
    [SerializeField] private Slider timerSlider;

    [Header("Wave Settings")]
    [SerializeField] private int baseCount = 2;
    [SerializeField] private int levelStartAdd = 2;
    [SerializeField] private int baseWaveAdd = 0;
    [SerializeField] private int levelWaveAdd = 1;

    private float timer;
    private bool battleFinished;
    private float waveTimer;
    private int waveIndex;
    private int currentWaveCount;
    private GameObject spawnedPortal;

    private void Start()
    {
        timer = battleDuration;

        if (timerSlider != null)
        {
            timerSlider.maxValue = battleDuration;
            timerSlider.value = battleDuration;
        }

        waveIndex = 0;
        currentWaveCount = GetWaveCount(waveIndex);
        waveTimer = waveInterval;

        if (spawner != null)
        {
            spawner.StartSpawning();
            // Сразу спавним первую волну (маленькую)
            spawner.SpawnWaveWithTypes(waveIndex, currentWaveCount);

            // Готовимся к следующей волне
            waveIndex++;
            currentWaveCount = GetWaveCount(waveIndex);
            waveTimer = waveInterval; // теперь таймер до следующей волны
        }
    }

    private void Update()
    {
        if (battleFinished) return;

        timer -= Time.deltaTime;
        if (timer < 0) timer = 0;
        UpdateUI();

        if (spawner != null && !battleFinished)
        {
            waveTimer -= Time.deltaTime;
            if (waveTimer <= 0)
            {
                // Спавним волну с учётом типов врагов
                spawner.SpawnWaveWithTypes(waveIndex, currentWaveCount);

                waveIndex++;
                currentWaveCount = GetWaveCount(waveIndex);
                waveTimer = waveInterval;
            }
        }

        if (timer <= 0)
        {
            EndBattle();
        }
    }

    private int GetWaveCount(int wave)
    {
        int level = playerSettings != null ? (int)playerSettings.Level : 1;
        return baseCount + level * levelStartAdd + wave * (baseWaveAdd + level * levelWaveAdd);
    }

    private void UpdateUI()
    {
        if (timerSlider != null) timerSlider.value = timer;
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    private void EndBattle()
    {
        battleFinished = true;
        if (spawner != null) spawner.StopSpawning();

        bool isPlayerAlive = playerHealth != null && !playerHealth.IsDead;

        if (isPlayerAlive)
        {
            Debug.Log("Battle finished — player alive! Loading Anomaly...");
            SceneManager.LoadScene("Anomaly");
        }
        else
        {
            Debug.Log("Battle finished — player dead! Loading MainMenu...");
            SceneManager.LoadScene("MainMenu");
        }

        // Спавним портал
        if (portalPrefab != null && spawnedPortal == null)
        {
            spawnedPortal = Instantiate(portalPrefab, portalSpawnPosition, Quaternion.identity);
            Debug.Log($"Портал заспавнен в позиции: {portalSpawnPosition}");
        }

        Debug.Log("Бой закончился! Ожидаем входа в портал...");
        enabled = false;
    }
}