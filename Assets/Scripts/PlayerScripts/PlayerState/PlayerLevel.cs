using UnityEngine;
using UnityEngine.UI;

public class PlayerLevel : MonoBehaviour
{
    [Header("Настройки уровней")]
    [SerializeField] private int startLevel = 1;
    [SerializeField] private int startKarma = 0;

    [Header("Формула опыта")]
    [SerializeField] private int baseKarmaForLevel = 10;  // Карма для 1 уровня
    [SerializeField] private float karmaMultiplier = 1.5f; // Множитель для каждого следующего уровня

    [Header("UI")]
    [SerializeField] private Slider levelSlider;           // Слайдер прогресса
    [SerializeField] private TMPro.TMP_Text levelText;     // Текст уровня (опционально)

    private int currentLevel;
    private int currentKarma;
    private int karmaForNextLevel;

    public int CurrentLevel => currentLevel;
    public int CurrentKarma => currentKarma;

    private void Start()
    {
        currentLevel = startLevel;
        currentKarma = startKarma;
        karmaForNextLevel = GetKarmaForLevel(currentLevel);

        UpdateUI();
        Debug.Log($"📊 Игрок начал с {currentLevel} уровня. Нужно {karmaForNextLevel} кармы для следующего уровня.");
    }

    // Добавить карму (вызывается из KarmaDrop)
    public void AddKarma(int amount)
    {
        currentKarma += amount;
        Debug.Log($"➕ Добавлено {amount} кармы. Всего: {currentKarma}");

        // Проверяем, можно ли повысить уровень
        CheckLevelUp();

        UpdateUI();
    }

    // Проверка и повышение уровня
    private void CheckLevelUp()
    {
        while (currentKarma >= karmaForNextLevel)
        {
            // Повышаем уровень
            currentKarma -= karmaForNextLevel;
            currentLevel++;
            karmaForNextLevel = GetKarmaForLevel(currentLevel);

            Debug.Log($"⬆️ УРОВЕНЬ ПОВЫШЕН! Текущий уровень: {currentLevel}");
            Debug.Log($"📊 Осталось кармы: {currentKarma}. Нужно для следующего: {karmaForNextLevel}");

            // Здесь можно вызвать событие повышения уровня
            OnLevelUp();
        }
    }

    // Формула: сколько кармы нужно для указанного уровня
    private int GetKarmaForLevel(int level)
    {
        // Например: уровень 1 = 10, 2 = 15, 3 = 22, 4 = 33 и т.д.
        return Mathf.RoundToInt(baseKarmaForLevel * Mathf.Pow(karmaMultiplier, level - 1));
    }

    // Событие при повышении уровня
    private void OnLevelUp()
    {
        // Можно обновить PlayerSettings или сделать другие действия
        PlayerSettings settings = GetComponent<PlayerSettings>();
        if (settings != null)
        {
            // Если у тебя есть метод для обновления уровня в PlayerSettings
            // settings.SetLevel(currentLevel);
        }

        // Анимация, звук, эффекты...
        Debug.Log($"🎉 УРОВЕНЬ {currentLevel} ДОСТИГНУТ!");
    }

    // Обновление UI
    private void UpdateUI()
    {
        if (levelSlider != null)
        {
            // Прогресс: от 0 до 1 (сколько накоплено к следующему уровню)
            float progress = (float)currentKarma / karmaForNextLevel;
            levelSlider.value = Mathf.Clamp01(progress);
        }

        if (levelText != null)
        {
            levelText.text = $"Уровень {currentLevel}\n{currentKarma}/{karmaForNextLevel}";
        }
    }

    // Получить прогресс для внешнего использования
    public float GetLevelProgress()
    {
        return Mathf.Clamp01((float)currentKarma / karmaForNextLevel);
    }

    // Для отладки
    private void OnDrawGizmos()
    {
        // Можно визуализировать в редакторе
    }
}