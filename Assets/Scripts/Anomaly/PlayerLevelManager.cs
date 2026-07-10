using UnityEngine;
using UnityEngine.UI;

public class PlayerLevelManager : MonoBehaviour
{
    [Header("Настройки системы")]
    [SerializeField] private int baseKarmaForLevel = 10;
    [SerializeField] private float karmaMultiplier = 1.5f;

    [Header("UI")]
    [SerializeField] private Slider levelSlider;

    private PlayerSettings playerSettings;
    private int currentKarma = 0;

    private void Awake()
    {
        playerSettings = GetComponent<PlayerSettings>();

        if (playerSettings == null)
        {
            Debug.LogError("❌ PlayerSettings не найден!");
        }
    }

    private void Start()
    {
        if (playerSettings != null)
        {
            Debug.Log($"📊 Текущий уровень: {playerSettings.playerLvl}");
            UpdateUI();
        }
    }

    // Добавить карму (вызывается из KarmaDrop)
    public void AddKarma(int amount)
    {
        if (playerSettings == null)
        {
            Debug.LogError("❌ Не могу добавить карму: PlayerSettings = null!");
            return;
        }

        currentKarma += amount;
        Debug.Log($"➕ Добавлено {amount} кармы. Всего: {currentKarma}");

        CheckLevelUp();
        UpdateUI();
    }

    // Проверка и повышение уровня
    private void CheckLevelUp()
    {
        if (playerSettings == null) return;

        int needed = GetKarmaForLevel(playerSettings.playerLvl);

        while (currentKarma >= needed)
        {
            currentKarma -= needed;
            IncreaseLevel();
            needed = GetKarmaForLevel(playerSettings.playerLvl);
        }
    }

    // Повысить уровень
    public void IncreaseLevel()
    {
        if (playerSettings == null) return;

        playerSettings.NewLevel();
        Debug.Log($"⬆ УРОВЕНЬ ПОВЫШЕН до: {playerSettings.playerLvl}");
    }

    // Установить уровень (без рефлексии!)
    public void SetLevel(int newLevel)
    {
        if (playerSettings == null) return;
        playerSettings.SetLevel(newLevel);
    }

    // Получить текущий уровень
    public int GetCurrentLevel()
    {
        return playerSettings != null ? playerSettings.playerLvl : 1;
    }

    // Получить прогресс до следующего уровня (0-1)
    public float GetProgress()
    {
        if (playerSettings == null) return 0f;

        int needed = GetKarmaForLevel(playerSettings.playerLvl);
        return needed > 0 ? Mathf.Clamp01((float)currentKarma / needed) : 0f;
    }

    // Сколько кармы нужно для уровня
    private int GetKarmaForLevel(int level)
    {
        return Mathf.RoundToInt(baseKarmaForLevel * Mathf.Pow(karmaMultiplier, level - 1));
    }

    // Обновление UI
    private void UpdateUI()
    {
        if (playerSettings == null) return;
        if (levelSlider == null) return;

        float progress = GetProgress();
        levelSlider.value = progress;

        Debug.Log($"📊 Слайдер обновлён: {progress * 100:F0}%");
    }

    // Получить текущую карму
    public int GetCurrentKarma()
    {
        return currentKarma;
    }
}