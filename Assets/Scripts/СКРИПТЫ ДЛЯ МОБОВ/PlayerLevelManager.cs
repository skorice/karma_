using UnityEngine;
using UnityEngine.UI;

public class PlayerLevelManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerSettings playerSettings;

    [Header("UI")]
    [SerializeField] private Slider levelSlider;

    [Header("Level System")]
    [SerializeField] private int baseXP = 10;
    [SerializeField] private float xpMultiplier = 1.5f;

    private int currentXP;

    private void Start()
    {
        if (playerSettings == null)
        {
            Debug.LogError("PlayerSettings не назначен!");
            return;
        }

        var levelSliderGO = GameObject.FindGameObjectWithTag("LVLTEXT");
        if (levelSliderGO != null)
        {
            levelSlider = levelSliderGO.GetComponent<Slider>();
        }
        
        // Попробуем найти слайдер, если он не назначен в инспекторе
        if (levelSlider == null)
        {
            // Используем FindFirstObjectByType
            levelSlider = FindFirstObjectByType<Slider>();
            if (levelSlider == null)
            {
                Debug.LogWarning("Level Slider не найден в сцене. UI-обновление будет пропущено.");
                return;
            }
        }

        UpdateUI();
    }

    public void AddKarma(int amount)
    {
        currentXP += amount;

        while (currentXP >= GetRequiredXP())
        {
            currentXP -= GetRequiredXP();
            
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayLevelUp();
            }
            
            playerSettings.NewLevel();

            Debug.Log($"Уровень повышен! Теперь: {playerSettings.playerLvl}");
        }

        UpdateUI();
    }

    private int GetRequiredXP()
    {
        return Mathf.RoundToInt(
            baseXP * Mathf.Pow(xpMultiplier, playerSettings.playerLvl - 1)
        );
    }

    private void UpdateUI()
    {
        // Главная защита от NullReference
        if (levelSlider == null)
            return;

        levelSlider.maxValue = GetRequiredXP();
        levelSlider.value = currentXP;
    }

    public int GetCurrentXP()
    {
        return currentXP;
    }
}