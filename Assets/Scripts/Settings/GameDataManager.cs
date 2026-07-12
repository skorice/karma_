using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }

    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int currentKarma = 0;
    [SerializeField] private int currentLives = 3;

    public int CurrentLevel
    {
        get => currentLevel;
        set
        {
            currentLevel = value;
            Debug.Log($"📊 GameDataManager: CurrentLevel изменён на {value} (из стека: {StackTraceUtility.ExtractStackTrace()})");
        }
    }

    public int CurrentKarma
    {
        get => currentKarma;
        set
        {
            currentKarma = value;
            Debug.Log($"📊 GameDataManager: CurrentKarma изменён на {value}");
        }
    }

    public int CurrentLives
    {
        get => currentLives;
        set
        {
            currentLives = value;
            Debug.Log($"📊 GameDataManager: CurrentLives изменён на {value}");
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log($"✅ GameDataManager создан. Уровень: {CurrentLevel}");
        }
        else
        {
            Debug.Log("⚠️ GameDataManager уже существует, уничтожаю дубликат");
            Destroy(gameObject);
        }
    }

    public void ResetProgress()
    {
        CurrentLevel = 1;
        CurrentKarma = 0;
        CurrentLives = 3;
        Debug.Log("🔄 Прогресс сброшен");
    }
}