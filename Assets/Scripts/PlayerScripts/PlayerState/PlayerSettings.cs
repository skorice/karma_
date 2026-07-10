using UnityEngine;

public class PlayerSettings : MonoBehaviour
{
    [Header("Базовые настройки игрока")]
    [SerializeField] private float BASEmoveSpeed = 5f;
    [SerializeField] private float BASEattackPower = 1f;
    [SerializeField] private float BASEattackSpeed = 0.6f;
    [SerializeField] private float BASEplayerHealth = 100f;
    [SerializeField] private int BASElives = 3;

    // Баффы за уровни
    [Header("Настройки прокачки")]
    [SerializeField] private float BUFFmoveSpeed = 0.1f;
    [SerializeField] private float BUFFattackPower = 0.5f;
    [SerializeField] private float BUFFattackSpeed = 0.05f;
    [SerializeField] private float BUFFplayerHealth = 10f;

    [Header("Текущий уровень")]
    [SerializeField] private int currentLevel = 1;

    // Дебаффы (временные)
    private float _slowMultiplier = 1f;
    private float _attackPowerDebuff = 1f;

    public int playerLvl => currentLevel;
    public float MoveSpeed => (BASEmoveSpeed + (currentLevel - 1) * BUFFmoveSpeed) * _slowMultiplier;
    public float AttackPower => (BASEattackPower + (currentLevel - 1) * BUFFattackPower) * _attackPowerDebuff;
    public float AttackSpeed => BASEattackSpeed + (currentLevel - 1) * BUFFattackSpeed;
    public float Health => BASEplayerHealth + (currentLevel - 1) * BUFFplayerHealth;
    public float Level => currentLevel;
    public int Lives => BASElives;

    public float TakeDamage(float damage) { return Health - damage; }

    // 🔥 ИСПРАВЛЕНО: повышаем уровень
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

    // Методы для SnakeTrail
    public void ApplySnakeDebuff()
    {
        _slowMultiplier = 0.8f;
        _attackPowerDebuff = 0.9f;
        Debug.Log("Snake debuff applied: speed -20%, attack -10%");
    }

    public void RemoveSnakeDebuff()
    {
        _slowMultiplier = 1f;
        _attackPowerDebuff = 1f;
        Debug.Log("Snake debuff removed");
    }


    public void SetLevel(int newLevel)
    {
        currentLevel = Mathf.Max(1, newLevel);
        Debug.Log($"Уровень установлен на: {currentLevel}");
    }
}