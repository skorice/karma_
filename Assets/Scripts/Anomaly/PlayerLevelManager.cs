//ЭТО ВРЕМЕННАЯ ЗАШЛУШКА ЧТОБЫ ПРОВЕРИТЬ РАБОТАЕТ ЛИ ПЕЩЕРА. ПОТОМ ЗАМЕНИТЬ НА КОД ЮЛИ
using UnityEngine;
using System.Reflection;

public class PlayerLevelManager : MonoBehaviour
{
    private PlayerSettings playerSettings;

    private void Awake()
    {
        playerSettings = GetComponent<PlayerSettings>();
        if (playerSettings == null)
        {
            Debug.LogError("PlayerSettings не найден!");
        }
    }

    public void IncreaseLevel()
    {
        if (playerSettings == null) return;

        var levelField = typeof(PlayerSettings).GetField("currentLevel", 
            BindingFlags.NonPublic | BindingFlags.Instance);
        
        if (levelField != null)
        {
            int currentLevel = (int)levelField.GetValue(playerSettings);
            currentLevel++;
            levelField.SetValue(playerSettings, currentLevel);
            Debug.Log($"⬆Уровень повышен до: {currentLevel}");
        }
    }

    public void SetLevel(int newLevel)
    {
        if (playerSettings == null) return;
        
        newLevel = Mathf.Max(1, newLevel);
        
        var levelField = typeof(PlayerSettings).GetField("currentLevel", 
            BindingFlags.NonPublic | BindingFlags.Instance);
        
        if (levelField != null)
        {
            levelField.SetValue(playerSettings, newLevel);
            Debug.Log($"Уровень установлен на: {newLevel}");
        }
    }

    public int GetCurrentLevel()
    {
        if (playerSettings == null) return 1;
        
        var levelField = typeof(PlayerSettings).GetField("currentLevel", 
            BindingFlags.NonPublic | BindingFlags.Instance);
        
        if (levelField != null)
        {
            return (int)levelField.GetValue(playerSettings);
        }
        return 1;
    }
}