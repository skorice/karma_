using UnityEngine;
using UnityEngine.SceneManagement;

public class CaveManager : MonoBehaviour
{
    public static CaveManager Instance;

    [Header("Scenes")]
    [SerializeField] private string fightScene = "KoryArena";
    [SerializeField] private string[] caveScenes;

    private int currentCave = -1;
    private int currentCycle = 1;

    public int CurrentCycle => currentCycle;
    public int CurrentCave => currentCave;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnBattleComplete()
    {
        currentCave = 0;
        SceneManager.LoadScene(caveScenes[currentCave]);
    }

    public void LoadNextCave()
    {
        currentCave++;

        if (currentCave >= caveScenes.Length)
        {
            Debug.LogWarning("Все пещеры пройдены.");
            return;
        }

        SceneManager.LoadScene(caveScenes[currentCave]);
    }

    public void ReturnToFight()
    {
        currentCave = -1;

        if (currentCycle < 3)
            currentCycle++;

        SceneManager.LoadScene(fightScene);
    }

    public bool IsFinalCycle()
    {
        return currentCycle == 3;
    }

    public bool IsLastCave()
    {
        return currentCave == caveScenes.Length - 1;
    }

    public void ResetGame()
    {
        currentCycle = 1;
        currentCave = -1;
        BuffScoreManager.Instance.ResetScore();
    }
}
