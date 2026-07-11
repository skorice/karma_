using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndManager : MonoBehaviour
{
    public Button stayButton;
    public Button quitButton;
    public string arenaScene = "KoryArena";
    public string cutsceneScene = "Cutscene";
    
    void Start()
    {
        stayButton.onClick.AddListener(StayInGame);
        quitButton.onClick.AddListener(QuitToCutscene);
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    private void StayInGame()
    {
        SceneManager.LoadScene(arenaScene);
    }
    
    private void QuitToCutscene()
    {
        // Проверяем есть ли сцена с катсценой
        if (Application.CanStreamedLevelBeLoaded(cutsceneScene))
            SceneManager.LoadScene(cutsceneScene);
        else
        {
            // Если катсцены нет - просто выходим
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}