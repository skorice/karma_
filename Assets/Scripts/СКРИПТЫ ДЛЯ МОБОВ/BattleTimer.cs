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

    [Header("UI")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Slider timerSlider;

    [Header("Spawn")]
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private float waveInterval = 25f;

    [Header("Wave Settings")]
    [Tooltip("Базовое количество врагов в первой волне")]
    [SerializeField] private int baseCount = 2;

    [Tooltip("Дополнительные враги за каждый уровень игрока")]
    [SerializeField] private int levelStartAdd = 2;

    [Tooltip("Дополнительные враги за каждую волну (не зависит от уровня)")]
    [SerializeField] private int baseWaveAdd = 0;

    [Tooltip("Дополнительные враги за каждую волну, умноженные на уровень")]
    [SerializeField] private int levelWaveAdd = 1;

    [Header("Portal")]
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private Transform portalSpawnPosition;
    [SerializeField] private string nextSceneName = "Cave1";

    private float timer;
    private float waveTimer;

    private bool battleFinished;

    private int waveIndex;
    private int currentWaveCount;

    private PlayerSettings playerSettings;
    private GameObject spawnedPortal;


    private void Start()
    {
        // Бой только на арене
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

        waveIndex = 0;
        currentWaveCount = GetWaveCount(waveIndex);
        waveTimer = waveInterval;

        StartBattle();
    }


    private void StartBattle()
    {
        if (spawner == null)
            return;

        spawner.StartSpawning();
        spawner.SpawnWaveWithTypes(waveIndex, currentWaveCount);

        waveIndex++;
        currentWaveCount = GetWaveCount(waveIndex);
    }


    private void Update()
    {
        if (battleFinished)
            return;


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


    private int GetWaveCount(int wave)
    {
        int level = playerSettings != null 
            ? (int)playerSettings.Level 
            : 1;

        return baseCount 
            + level * levelStartAdd 
            + wave * (baseWaveAdd + level * levelWaveAdd);
    }


    private void UpdateUI()
    {
        if (timerSlider != null)
            timerSlider.value = timer;


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
            Debug.LogError(" Portal Prefab не назначен!");
            return;
        }


        if (portalSpawnPosition == null)
        {
            Debug.LogError(
                "Не назначена позиция появления портала!"
            );
            return;
        }


        spawnedPortal = Instantiate(
            portalPrefab,
            portalSpawnPosition.position,
            Quaternion.identity
        );

        Debug.Log(
            $" Портал заспавнен: {portalSpawnPosition.position}"
        );
    }


    private void OnDrawGizmosSelected()
    {
        if (portalSpawnPosition != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(
                portalSpawnPosition.position,
                1f
            );
        }
    }
}