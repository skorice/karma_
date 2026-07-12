using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private PlayerSettings settings;
    [SerializeField] private HealthBar healthBar;

    [Header("Respawn")]
    [SerializeField] private Transform respawnPoint; // если не назначен — центр (0,0,0)

    private float _currentHealth;
    private int _currentLives;
    public bool IsDead => _currentHealth <= 0;

    public float MaxHealth => settings != null ? settings.Health : 100f;
    public float CurrentHealth => _currentHealth;
    public int CurrentLives => _currentLives;

    public event System.Action<float, float> OnHealthChanged;
    public event System.Action<int> OnLivesChanged;

    // Компоненты для отключения на время паузы
    private PlayerMove playerMove;
    private PlayerFight playerFight;

    private void Awake()
    {
        if (settings == null)
            settings = GetComponent<PlayerSettings>();

        if (settings == null)
            Debug.LogError("PlayerSettings не найден! Назначьте его в инспекторе.");

        playerMove = GetComponent<PlayerMove>();
        playerFight = GetComponent<PlayerFight>();
    }

    private void Start()
    {
        if (settings == null)
        {
            Debug.LogError("PlayerSettings отсутствует! Скрипт отключён.");
            enabled = false;
            return;
        }

        _currentLives = settings.Lives;
        _currentHealth = MaxHealth;

        if (healthBar != null)
            OnHealthChanged += healthBar.UpdateHealth;

        OnHealthChanged?.Invoke(_currentHealth, MaxHealth);
        OnLivesChanged?.Invoke(_currentLives);
    }

    public void TakeDamage(float damage)
    {
        if (_currentHealth <= 0) return;

        _currentHealth = Mathf.Max(0, _currentHealth - damage);
        OnHealthChanged?.Invoke(_currentHealth, MaxHealth);

        if (settings != null)
            settings.FlashRed();

        if (_currentHealth <= 0)
            Die();
    }

    public void HealToMax()
    {
        _currentHealth = MaxHealth;
        OnHealthChanged?.Invoke(_currentHealth, MaxHealth);
        Debug.Log($"❤️ Здоровье восстановлено до {_currentHealth}");
    }

    private void Die()
    {
        Debug.Log("💀 Игрок умер!");

        _currentLives--;
        OnLivesChanged?.Invoke(_currentLives);

        if (_currentLives > 0)
        {
            // Запускаем перерождение с задержкой
            StartCoroutine(RespawnCoroutine());
        }
        else
        {
            GameOver();
        }
    }

    private IEnumerator RespawnCoroutine()
    {
        // Отключаем управление и бой на время паузы
        if (playerMove != null) playerMove.enabled = false;
        if (playerFight != null) playerFight.enabled = false;

        // Можно сделать игрока невидимым или отключить спрайт (опционально)
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        // Ждём 2 секунды
        yield return new WaitForSeconds(2f);

        // Телепортируем в центр (или в respawnPoint)
        Vector3 spawnPos = respawnPoint != null ? respawnPoint.position : Vector3.zero;
        transform.position = spawnPos;

        // Восстанавливаем здоровье
        _currentHealth = MaxHealth;
        OnHealthChanged?.Invoke(_currentHealth, MaxHealth);

        Debug.Log($"🔄 Перерождение! Осталось жизней: {_currentLives}");

        // Возвращаем видимость и управление
        if (sr != null) sr.enabled = true;
        if (playerMove != null) playerMove.enabled = true;
        if (playerFight != null) playerFight.enabled = true;
    }

    private void GameOver()
    {
        StartCoroutine(LoadMainMenuAfterDelay(2f));
    }

    private IEnumerator LoadMainMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("MainMenu");
    }

    public void AddLives(int count)
    {
        if (settings == null) return;
        _currentLives = settings.newLive(count);
        OnLivesChanged?.Invoke(_currentLives);
    }

    public void RefreshHealthStats()
    {
        if (settings == null) return;
        float newMax = MaxHealth;
        _currentHealth = Mathf.Min(_currentHealth, newMax);
        OnHealthChanged?.Invoke(_currentHealth, newMax);
    }

    private void OnDestroy()
    {
        if (healthBar != null)
            OnHealthChanged -= healthBar.UpdateHealth;
    }
}