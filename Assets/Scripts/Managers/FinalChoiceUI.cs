using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalChoiceUI : MonoBehaviour
{
    public static FinalChoiceUI Instance;

    [SerializeField] private GameObject panel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    private void Start()
    {
        // Если панель по какой-то причине активна, выключаем
        if (panel != null && panel.activeSelf)
        {
            panel.SetActive(false);
        }
    }

    public void Show()
    {
        if (panel == null)
        {
            Debug.LogError("Panel не назначен в FinalChoiceUI!");
            return;
        }

        panel.SetActive(true);
        Time.timeScale = 0f;
        Debug.Log("🎮 Финальная панель выбора показана!");
    }

    public void Hide()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
        Time.timeScale = 1f;
    }

    // Кнопка "Остаться в игре" (Продолжить)
    public void ContinueGame()
    {
        Debug.Log("🔄 Игрок выбрал 'Остаться в игре'!");
        Time.timeScale = 1f;
        Hide();
        
        if (CaveManager.Instance != null)
        {
            // Возвращаемся на арену KoryArena
            CaveManager.Instance.ReturnToKoryArena();
        }
        else
        {
            Debug.LogError("CaveManager.Instance не найден!");
        }
    }

    // Кнопка "Выйти из игры" (Закончить)
    public void StopGame()
    {
        Debug.Log("🚪 Игрок выбрал 'Выйти из игры'!");
        Time.timeScale = 1f;
        Hide();
        
        // Переход в финальную сцену
        SceneManager.LoadScene("Final");
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
        Time.timeScale = 1f;
    }
}