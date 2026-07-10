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
    private bool battleFinished;
    private float waveTimer;
    private int waveIndex;
    private int currentWaveCount;
    private GameObject spawnedPortal;
    private PlayerSettings playerSettings;

    private void Start()
    {
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

        if (spawner != null)
        {
            spawner.StartSpawning();
            spawner.SpawnWaveWithTypes(waveIndex, currentWaveCount);

            waveIndex++;
            currentWaveCount = GetWaveCount(waveIndex);
            waveTimer = waveInterval;
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
                spawner.SpawnWaveWithTypes(waveIndex, currentWaveCount);
                waveIndex++;
                currentWaveCount = GetWaveCount(waveIndex);
                waveTimer = waveInterval;
            }
        }

        if (timer <= 0 && !battleFinished)
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

        Debug.Log("⚔️ Битва окончена! Спавним портал...");
        SpawnPortal();

        enabled = false;
    }

    private void SpawnPortal()
    {
        if (portalPrefab == null)
        {
            Debug.LogError("❌ Portal Prefab не назначен в инспекторе!");
            return;
        }

        if (portalSpawnPosition == null)
        {
            Debug.LogWarning("⚠️ Portal Spawn Position не назначен! Использую позицию игрока.");

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                GameObject tempPos = new GameObject("PortalSpawnPosition");
                tempPos.transform.position = player.transform.position;
                portalSpawnPosition = tempPos.transform;
            }
            else
            {
                Debug.LogError("❌ Не найдена позиция для спавна портала!");
                return;
            }
        }

        spawnedPortal = Instantiate(
            portalPrefab,
            portalSpawnPosition.position,
            Quaternion.identity
        );

        Debug.Log($"🌀 Портал заспавнен в позиции: {portalSpawnPosition.position}");

        PortalTrigger portalTrigger = spawnedPortal.GetComponent<PortalTrigger>();
        if (portalTrigger == null)
        {
            portalTrigger = spawnedPortal.AddComponent<PortalTrigger>();
        }
        portalTrigger.Initialize(nextSceneName);
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