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
    public float MoveSpeed => BASEmoveSpeed + (currentLevel - 1) * BUFFmoveSpeed;
    public float AttackPower => BASEattackPower + (currentLevel - 1) * BUFFattackPower;
    public float AttackSpeed => BASEattackSpeed + (currentLevel - 1) * BUFFattackSpeed;
    public float Health => BASEplayerHealth + (currentLevel - 1) * BUFFplayerHealth;
    public float Level =>currentLevel;
    public int Lives => BASElives; 

    public float TakeDamage(float damage) { return Health - damage; }
    public float NewLevel()
    {
        return BASEplayerHealth + (Level - 1) * BUFFplayerHealth;
    }

    public void LoseLevel()
    {
        currentLevel = Mathf.Max(1, currentLevel - 1);
    }
    public int newLive(int count)
    {
        return BASElives + count;
    }
}
