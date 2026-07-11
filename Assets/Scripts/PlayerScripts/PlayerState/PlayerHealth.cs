using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private PlayerSettings settings;
    [SerializeField] private HealthBar healthBar;

    private float _currentHealth;
    private int _currentLives;
    public bool IsDead => _currentHealth <= 0;

    public float MaxHealth => settings != null ? settings.Health : 100f;
    public float CurrentHealth => _currentHealth;
    public int CurrentLives => _currentLives;

    public event System.Action<float, float> OnHealthChanged;
    public event System.Action<int> OnLivesChanged;

    private void Awake()
    {
        if (settings == null)
            settings = GetComponent<PlayerSettings>();

        if (settings == null)
            Debug.LogError("PlayerSettings не найден в инспекторе.");
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

        // Красная вспышка на игроке
        if (settings != null)
            settings.FlashRed();

        if (_currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        Debug.Log("💀 Игрок умер!");

        _currentLives--;
        OnLivesChanged?.Invoke(_currentLives);

        if (_currentLives > 0)
        {
            _currentHealth = MaxHealth;
            OnHealthChanged?.Invoke(_currentHealth, MaxHealth);
            Debug.Log($"🔄 Перерождение! Осталось жизней: {_currentLives}");
            // Здесь можно вызвать респавн на спавн-точку
        }
        else
        {
            GameOver();
        }
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