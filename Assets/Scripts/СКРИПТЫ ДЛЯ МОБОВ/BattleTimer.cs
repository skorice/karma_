using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleTimer : MonoBehaviour
{
    [Header("Arena")]
    [SerializeField] private string arenaSceneName = "KoryArena";

    [Header("Timer")]
    [SerializeField] private float battleDuration = 90f;

    [Header("Start Delay")]
    [SerializeField] private float startDelay = 3f;

    [Header("UI")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Slider timerSlider;

    [Header("Spawn")]
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private float waveInterval = 25f;

    [Header("Wave Settings")]
    [SerializeField] private int baseCount = 2;
    [SerializeField] private int levelStartAdd = 2;
    [SerializeField] private int baseWaveAdd = 0;
    [SerializeField] private int levelWaveAdd = 1;

    [Header("Portal")]
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private Transform portalSpawnPosition;
    [SerializeField] private string nextSceneName = "Anomaly";

    private float timer;
    private float waveTimer;
    private bool battleFinished;
    private int waveIndex;
    private int currentWaveCount;
    private PlayerSettings playerSettings;
    private GameObject spawnedPortal;

    // Для задержки
    private bool isDelaying;
    private float delayTimer;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name != arenaSceneName)
        {
            enabled = false;
            return;
        }

        timer = battleDuration;

        if (timerSlider != null)
        {
            timerSlider.maxValue = battleDuration;
            timerSlider.value = battleDuration;
        }

        playerSettings = FindFirstObjectByType<PlayerSettings>();

        // Начинаем задержку
        isDelaying = true;
        delayTimer = startDelay;
        UpdateTimerDisplay(); // показываем начальный отсчёт
        Debug.Log($"⏳ Обратный отсчёт: {startDelay} секунд до начала боя");
    }

    private void Update()
    {
        if (battleFinished) return;

        // --- Фаза задержки ---
        if (isDelaying)
        {
            delayTimer -= Time.deltaTime;
            UpdateTimerDisplay(); // обновляем текст обратного отсчёта

            if (delayTimer <= 0)
            {
                isDelaying = false;
                timerText.text = "GO!";
                StartBattle();
                Debug.Log("⚔️ Бой начался!");
                // Через 0.5 секунды убираем "GO!" и показываем нормальное время
                Invoke(nameof(ResetTimerDisplay), 0.5f);
            }
            return;
        }

        // --- Основной цикл боя ---
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            timer = 0;
            UpdateUI();
            EndBattle();
            return;
        }

        UpdateUI();

        if (spawner != null)
        {
            waveTimer -= Time.deltaTime;
            if (waveTimer <= 0)
            {
                spawner.SpawnWaveWithTypes(waveIndex, currentWaveCount);
                waveIndex++;
                currentWaveCount = GetWaveCount(waveIndex);
                waveTimer = waveInterval;
            }
        }
    }

    private void StartBattle()
    {
        if (spawner == null) return;
        AudioManager.Instance.PlayStartFight();

        AudioManager.Instance.PlayMusicBattle();
        waveIndex = 0;
        currentWaveCount = GetWaveCount(waveIndex);
        waveTimer = waveInterval;

        spawner.StartSpawning();
        spawner.SpawnWaveWithTypes(waveIndex, currentWaveCount);

        waveIndex++;
        currentWaveCount = GetWaveCount(waveIndex);
    }

    private int GetWaveCount(int wave)
    {
        int level = playerSettings != null ? (int)playerSettings.Level : 1;
        return baseCount
             + level * levelStartAdd
             + wave * (baseWaveAdd + level * levelWaveAdd);
    }

    // Обновление UI (основной таймер)
    private void UpdateUI()
    {
        if (timerSlider != null)
            timerSlider.value = timer;

        if (timerText != null && !isDelaying)
        {
            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    // Обновление текста во время обратного отсчёта
    private void UpdateTimerDisplay()
    {
        if (timerText == null) return;

        if (isDelaying)
        {
            int seconds = Mathf.CeilToInt(delayTimer);
            timerText.text = seconds > 0 ? $"{seconds}" : "GO!";
        }
    }

    // Сброс текста после "GO!" на нормальный таймер
    private void ResetTimerDisplay()
    {
        if (!isDelaying && timerText != null)
        {
            UpdateUI();
        }
    }

    private void EndBattle()
    {
        battleFinished = true;
        if (spawner != null)
            spawner.StopSpawning();

        Debug.Log("Битва окончена. Спавн портала...");
        SpawnPortal();
        enabled = false;
    }

    private void SpawnPortal()
    {
        if (portalPrefab == null)
        {
            Debug.LogError("❌ Portal Prefab не назначен!");
            return;
        }

        if (portalSpawnPosition == null)
        {
            Debug.LogError("❌ Не назначена позиция появления портала!");
            return;
        }
        AudioManager.Instance.PlayPortal();
        spawnedPortal = Instantiate(portalPrefab, portalSpawnPosition.position, Quaternion.identity);
        Debug.Log($"🌀 Портал заспавнен: {portalSpawnPosition.position}");
    }

    private void OnDrawGizmosSelected()
    {
        if (portalSpawnPosition != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(portalSpawnPosition.position, 1f);
        }
    }
}