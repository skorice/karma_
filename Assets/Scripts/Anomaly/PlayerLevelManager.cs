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

        if (levelSlider == null)
        {
            Debug.LogError("Level Slider не назначен!");
            return;
        }

        UpdateUI();
    }

    public void AddKarma(int amount)
    {
        currentXP += amount;

        while (currentXP >= GetRequiredXP())
        {
            currentXP -= GetRequiredXP();

            playerSettings.NewLevel();

            Debug.Log($"Уровень повышен! Теперь: {playerSettings.playerLvl}");
        }

        UpdateUI();
    }

    private int GetRequiredXP()
    {
        return Mathf.RoundToInt(baseXP *
               Mathf.Pow(xpMultiplier, playerSettings.playerLvl - 1));
    }

    private void UpdateUI()
    {
        levelSlider.maxValue = GetRequiredXP();
        levelSlider.value = currentXP;
    }

    public int GetCurrentXP()
    {
        return currentXP;
    }
}