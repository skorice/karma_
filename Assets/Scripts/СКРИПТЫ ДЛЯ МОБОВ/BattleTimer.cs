using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleTimer : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] private float battleDuration = 90f;

    [Header("UI")]
    [SerializeField] private TMP_Text timerText;

    [Header("References")]
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private Vector3 portalSpawnPosition = new Vector3(90f, 90f, 0f);

    [Header("TimeSlider")]
    [SerializeField] private Slider timerSlider;

    private float timer;
    private bool battleFinished;
    private GameObject spawnedPortal;

    private void Start()
    {
        timer = battleDuration;

        if (timerSlider != null)
        {
            timerSlider.maxValue = battleDuration;
            timerSlider.value = battleDuration;
        }

        if (spawner != null)
        {
            spawner.StartSpawning();
        }
    }

    private void Update()
    {
        if (battleFinished)
            return;

        timer -= Time.deltaTime;

        if (timer < 0)
            timer = 0;

        UpdateUI();

        if (timer <= 0)
        {
            EndBattle();
        }
    }

    private void UpdateUI()
    {
        if (timerSlider != null)
        {
            timerSlider.value = timer;
        }

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