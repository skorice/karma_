using UnityEngine;
using UnityEngine.SceneManagement;

public class CaveManager : MonoBehaviour
{
    public static CaveManager Instance;

    [Header("Настройки фазы")]
    public int maxAttempts = 4;
    private int currentAttempts = 0;
    
    [Header("Настройки сцен")]
    public string KoryArena = "KoryArena";
    public string Anomaly = "Anomaly";
    
    [Header("Очки удачи")]
    [SerializeField] private int luckPoints = 0;
    public int LuckPoints => luckPoints;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        StartSearchPhase();
    }

    public void StartSearchPhase()
    {
        if (currentAttempts >= maxAttempts)
        {
            Debug.Log("Попытки кончились! Переход к выбору баффов.");
            CalculateBuffs();
            return;
        }

        currentAttempts++;
        Debug.Log($"Фаза поиска. Попытка {currentAttempts} из {maxAttempts}");
        
        AnomalySpawner spawner = FindObjectOfType<AnomalySpawner>();
        if (spawner != null) spawner.GenerateNewLocation();
    }

    public void CheckPlayerChoice(bool hasAnomaly, bool playerChoseLeft)
    {
        bool isCorrect = (hasAnomaly && playerChoseLeft) || (!hasAnomaly && !playerChoseLeft);

        if (isCorrect)
        {
            luckPoints = Mathf.Min(luckPoints + 1, 4);
            Debug.Log($" Верно! Очков удачи: {luckPoints}");
        }
        else
        {
            luckPoints = Mathf.Max(luckPoints - 1, 0);
            Debug.Log($" Неверно! Очков удачи: {luckPoints}");
        }

        //  ПЕРЕХОД В СЦЕНУ БОЯ
        Invoke(nameof(GoToKoryArena), 1f);
    }

    public void GoToKoryArena()
    {
        Debug.Log($"Переход в сцену боя: {KoryArena}");
        SceneManager.LoadScene(KoryArena);
    }

    public void GoToCaveScene()
    {
        Debug.Log($"Переход в сцену пещеры: {Anomaly}");
        SceneManager.LoadScene(Anomaly);
    }

    public int GetCurrentAttempts()
    {
        return currentAttempts;
    }

    private void CalculateBuffs()
    {
        int O = luckPoints;
        Debug.Log($"\n=== РАСЧЁТ БАФФОВ (Очков удачи: {O}) ===");

        if (O <= 2)
        {
            int rareChance = O * 20;
            RollBuff("Слот 1", rareChance, 0);
        }
        else
        {
            int epicChance = (O - 2) * 25;
            RollBuff("Слот 1", 0, epicChance);
        }

        if (O <= 1)
        {
            RollBuff("Слот 2", 0, 0);
        }
        else
        {
            int rareChance = (O - 1) * 20;
            RollBuff("Слот 2", rareChance, 0);
        }

        if (O <= 2)
        {
            RollBuff("Слот 3", 0, 0);
        }
        else
        {
            int rareChance = (O - 2) * 20;
            RollBuff("Слот 3", rareChance, 0);
        }

        //  ПОСЛЕ РАСЧЁТА БАФФОВ — БОЙ
        Invoke(nameof(GoToKoryArena), 1.5f);
    }

    private void RollBuff(string slotName, int rarePercent, int epicPercent)
    {
        int roll = Random.Range(0, 101);

        if (epicPercent > 0 && roll <= epicPercent)
        {
            Debug.Log($"{slotName}: ЭПИК (шанс {epicPercent}%)");
        }
        else if (rarePercent > 0 && roll <= rarePercent)
        {
            Debug.Log($"{slotName}: РЕДКИЙ (шанс {rarePercent}%)");
        }
        else
        {
            Debug.Log($"{slotName}: БАЗОВЫЙ");
        }
    }
}