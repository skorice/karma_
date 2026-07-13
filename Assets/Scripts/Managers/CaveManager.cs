using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CaveManager : MonoBehaviour
{
    public static CaveManager Instance;

    [Header("Scenes")]
    [SerializeField] private string fightScene = "KoryArena";
    [SerializeField] private string[] caveScenes;
    [SerializeField] private GameObject finalPanelPrefab;

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

    // Вызывается после боя, когда игрок входит в портал (старт цикла: пещера 0)
    public void OnBattleComplete()
    {
        currentCave = 0;
        SceneManager.LoadScene(caveScenes[currentCave], LoadSceneMode.Single);
        StartCoroutine(PlacePlayerInCave());
    }

    // Переход к следующей пещере
    public void LoadNextCave()
    {
        currentCave++;

        if (currentCave >= caveScenes.Length)
        {
            Debug.LogWarning("Все пещеры пройдены.");
            return;
        }

        SceneManager.LoadScene(caveScenes[currentCave], LoadSceneMode.Single);
        StartCoroutine(PlacePlayerInCave());
    }

    // Возврат на арену (если цикл не финальный)
    public void ReturnToFight()
    {
        currentCave = -1;

        if (currentCycle < 3)
            currentCycle++;

        SceneManager.LoadScene(fightScene, LoadSceneMode.Single);
        StartCoroutine(ActivateArenaAndPlacePlayer());
    }

    // Проверка: третий ли цикл
    public bool IsFinalCycle()
    {
        return currentCycle == 3;
    }

    // Проверка: последняя ли пещера
    public bool IsLastCave()
    {
        return currentCave == caveScenes.Length - 1;
    }

    // Показ финальной панели
    public void ShowFinalPanel()
    {
        Instantiate(finalPanelPrefab);
    }

    // Полный сброс игры
    public void ResetGame()
    {
        currentCycle = 1;
        currentCave = -1;
        BuffScoreManager.Instance.ResetScore();
    }

    // --- Перемещение игрока после загрузки пещеры ---
    private IEnumerator PlacePlayerInCave()
    {
        yield return null; // ждём 1 кадр, пока сцена прогрузится

        GameObject spawnObj = GameObject.FindWithTag("CaveSpawnPoint");
        if (spawnObj == null)
        {
            Debug.LogError("CaveSpawnPoint не найден в сцене! Убедись, что он есть и имеет тег.");
            yield break;
        }

        Transform spawnPoint = spawnObj.transform;
        PlayerSpawnManager.Instance.SpawnPlayer(spawnPoint.position, false);
    }

    // --- Перемещение игрока после загрузки арены ---
    private IEnumerator ActivateArenaAndPlacePlayer()
    {
        yield return null; // ждём загрузку

        Scene arena = SceneManager.GetSceneByName(fightScene);
        SceneManager.SetActiveScene(arena);

        Debug.Log("Сцена арены активирована: " + SceneManager.GetActiveScene().name);

        PlayerSpawnManager.Instance.SpawnPlayer(Vector3.zero, true);
    }
}
