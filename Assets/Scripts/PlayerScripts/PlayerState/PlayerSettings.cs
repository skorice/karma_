using UnityEngine;
using System.Collections;

public class PlayerSettings : MonoBehaviour
{
    [Header("Базовые настройки игрока")]
    [SerializeField] private float BASEmoveSpeed = 5f;
    [SerializeField] private float BASEattackPower = 1f;
    [SerializeField] private float BASEattackSpeed = 0.6f;
    [SerializeField] private float BASEplayerHealth = 100f;
    [SerializeField] private int BASElives = 3;

    [Header("Баффы за уровни")]
    [SerializeField] private float BUFFmoveSpeed = 0.1f;
    [SerializeField] private float BUFFattackPower = 0.5f;
    [SerializeField] private float BUFFattackSpeed = 0.05f;
    [SerializeField] private float BUFFplayerHealth = 10f;

    [Header("Текущий уровень")]
    [SerializeField] private int currentLevel = 1;

    // Дебаффы
    private float _slowMultiplier = 1f;
    private float _attackPowerDebuff = 1f;

    // Цветовые эффекты
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Color baseColor;
    private Coroutine flashCoroutine;

    // Публичные свойства (статы)
    public int playerLvl => currentLevel;
    public float MoveSpeed => (BASEmoveSpeed + (currentLevel - 1) * BUFFmoveSpeed) * _slowMultiplier;
    public float AttackPower => (BASEattackPower + (currentLevel - 1) * BUFFattackPower) * _attackPowerDebuff;
    public float AttackSpeed => BASEattackSpeed + (currentLevel - 1) * BUFFattackSpeed;
    public float Health => BASEplayerHealth + (currentLevel - 1) * BUFFplayerHealth;
    public float Level => currentLevel;
    public int Lives => BASElives;

    private void Start()
    {
        // 1. Загружаем уровень из глобального менеджера
        if (GameDataManager.Instance != null)
        {
            currentLevel = GameDataManager.Instance.CurrentLevel;
            Debug.Log($"📥 Уровень загружен из GameDataManager: {currentLevel}");
        }

        // 2. Инициализация спрайта для эффектов
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            baseColor = originalColor;
        }
    }

    // ----- Статы и прокачка -----
    public float TakeDamage(float damage) { return Health - damage; }

    public float NewLevel()
    {
        currentLevel++;
        // Сохраняем в глобальный менеджер
        if (GameDataManager.Instance != null)
            GameDataManager.Instance.CurrentLevel = currentLevel;

        Debug.Log($"⬆️ Уровень повышен до {currentLevel}");
        DebugStats();

        // ✅ Восстанавливаем здоровье до максимума
        PlayerHealth health = GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.HealToMax();
            Debug.Log("❤️ Здоровье восстановлено до максимума!");
        }

        return BASEplayerHealth + (currentLevel - 1) * BUFFplayerHealth;
    }

    public void LoseLevel()
    {
        currentLevel = Mathf.Max(1, currentLevel - 1);
        if (GameDataManager.Instance != null)
            GameDataManager.Instance.CurrentLevel = currentLevel;
        Debug.Log($"⬇️ Уровень понижен до {currentLevel}");
    }

    public void SetLevel(int newLevel)
    {
        currentLevel = Mathf.Max(1, newLevel);
        if (GameDataManager.Instance != null)
            GameDataManager.Instance.CurrentLevel = currentLevel;
        Debug.Log($"📤 Уровень установлен на {currentLevel}");
    }

    public int newLive(int count)
    {
        return BASElives + count;
    }

    // ----- Дебафф Наги (зелёный) -----
    public void ApplySnakeDebuff()
    {
        _slowMultiplier = 0.8f;
        _attackPowerDebuff = 0.9f;
        Debug.Log("🐍 Дебафф Наги: скорость -20%, атака -10%");

        if (spriteRenderer != null)
        {
            baseColor = new Color(0f, 1f, 0f, 1f);
            spriteRenderer.color = baseColor;
        }
    }

    public void RemoveSnakeDebuff()
    {
        _slowMultiplier = 1f;
        _attackPowerDebuff = 1f;
        Debug.Log("✅ Дебафф Наги снят");

        if (spriteRenderer != null)
        {
            baseColor = originalColor;
            spriteRenderer.color = baseColor;
        }
    }

    // ----- Красная вспышка при получении урона -----
    public void FlashRed()
    {
        if (spriteRenderer == null) return;
        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashRedCoroutine());
    }

    private IEnumerator FlashRedCoroutine()
    {
        Color targetColor = baseColor;
        spriteRenderer.color = new Color(1f, 0.2f, 0.2f, 1f);
        yield return new WaitForSeconds(0.15f);
        spriteRenderer.color = targetColor;
        flashCoroutine = null;
    }

    // ----- Отладка -----
    public void DebugStats()
    {
        Debug.Log($"📊 Уровень: {currentLevel}");
        Debug.Log($"🏃 Скорость: {MoveSpeed:F2}");
        Debug.Log($"⚔️ Сила атаки: {AttackPower:F2}");
        Debug.Log($"⏱️ Скорость атаки: {AttackSpeed:F2}");
        Debug.Log($"❤️ Здоровье: {Health:F0}");
    }
}