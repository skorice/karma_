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
        panel.SetActive(false);
    }

    public void Show()
    {
        panel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ContinueGame()
    {
        Time.timeScale = 1f;
        CaveManager.Instance.ReturnToFight();
    }

    public void StopGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Cutscene");
    }
}
