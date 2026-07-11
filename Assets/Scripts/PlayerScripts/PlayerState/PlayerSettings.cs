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

    private float _slowMultiplier = 1f;
    private float _attackPowerDebuff = 1f;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Color baseColor; // текущий "базовый" цвет (оригинал или зелёный)
    private Coroutine flashCoroutine;

    public int playerLvl => currentLevel;
    public float MoveSpeed => (BASEmoveSpeed + (currentLevel - 1) * BUFFmoveSpeed) * _slowMultiplier;
    public float AttackPower => (BASEattackPower + (currentLevel - 1) * BUFFattackPower) * _attackPowerDebuff;
    public float AttackSpeed => BASEattackSpeed + (currentLevel - 1) * BUFFattackSpeed;
    public float Health => BASEplayerHealth + (currentLevel - 1) * BUFFplayerHealth;
    public float Level => currentLevel;
    public int Lives => BASElives;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            baseColor = originalColor;
        }
    }

    public float TakeDamage(float damage) { return Health - damage; }

    public float NewLevel()
    {
        currentLevel++;
        return BASEplayerHealth + (currentLevel - 1) * BUFFplayerHealth;
    }

    public void LoseLevel()
    {
        currentLevel = Mathf.Max(1, currentLevel - 1);
    }

    public int newLive(int count)
    {
        return BASElives + count;
    }

    public void ApplySnakeDebuff()
    {
        _slowMultiplier = 0.8f;
        _attackPowerDebuff = 0.9f;
        Debug.Log("Snake debuff applied");

        if (spriteRenderer != null)
        {
            baseColor = new Color(0f, 1f, 0f, 1f); // зелёный, 30% прозрачности
            spriteRenderer.color = baseColor;
        }
    }

    public void RemoveSnakeDebuff()
    {
        _slowMultiplier = 1f;
        _attackPowerDebuff = 1f;
        Debug.Log("Snake debuff removed");

        if (spriteRenderer != null)
        {
            baseColor = originalColor;
            spriteRenderer.color = baseColor;
        }
    }

    // Красная вспышка при получении урона
    public void FlashRed()
    {
        if (spriteRenderer == null) return;
        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashRedCoroutine());
    }

    private IEnumerator FlashRedCoroutine()
    {
        // Сохраняем текущий базовый цвет (оригинал или зелёный)
        Color targetColor = baseColor;
        // Устанавливаем красный полупрозрачный
        spriteRenderer.color = new Color(1f, 0.2f, 0.2f, 1f);
        yield return new WaitForSeconds(0.15f);
        // Возвращаем базовый цвет
        spriteRenderer.color = targetColor;
        flashCoroutine = null;
    }

    public void SetLevel(int newLevel)
    {
        currentLevel = Mathf.Max(1, newLevel);
        Debug.Log($"Уровень установлен на: {currentLevel}");
    }
}