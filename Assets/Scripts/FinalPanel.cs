using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalPanel : MonoBehaviour
{
    public void OnExit()
    {
        Application.Quit();
    }

    public void OnRestart()
    {
        CaveManager.Instance.ResetGame();
        SceneManager.LoadScene("KoryArena");
    }
}
