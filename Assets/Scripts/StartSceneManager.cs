using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
{
    private DialogueController dialogueController;

    void Start()
    {
        // Используем FindFirstObjectByType вместо устаревшего FindObjectOfType
        dialogueController = FindFirstObjectByType<DialogueController>();
        
        // Запускаем диалог для стартовой сцены
        if (dialogueController != null)
        {
            dialogueController.StartDialogForScene("Start");
        }
    }

    void Update()
    {
        // Проверяем нажатие клавиши Пробел только если диалог завершен
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (dialogueController == null || dialogueController.IsDialogFinished())
            {
                PlayGame();
            }
        }
    }

    // Метод для перехода в арену (можно также вешать на кнопку)
    public void PlayGame()
    {
        SceneManager.LoadScene("KoryArena");
    }
    
    // Если нужен автоматический переход через N секунд
    public void AutoLoadArena(float delay = 3f)
    {
        Invoke(nameof(LoadArena), delay);
    }
    
    private void LoadArena()
    {
        SceneManager.LoadScene("KoryArena");
    }
}