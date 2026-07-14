using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CaveManager : MonoBehaviour
{
    public static CaveManager Instance;

    [Header("Scenes")]
    [SerializeField] private string fightScene = "KoryArena";
    [SerializeField] private string[] caveScenes;
    [SerializeField] private string finalScene = "Final";

    private int currentCave = -1;
    private int currentCycle = 1;

    private bool isLoadingScene = false;

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


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isLoadingScene = false;

        Debug.Log(
            $"📦 Сцена загружена: {scene.name} | Цикл {currentCycle} | Пещера {currentCave}"
        );


        if (scene.name == fightScene)
        {
            StartCoroutine(ActivateArenaAndPlacePlayer());
            return;
        }


        if (currentCave >= 0 &&
            currentCave < caveScenes.Length &&
            scene.name == caveScenes[currentCave])
        {
            StartCoroutine(PlacePlayerInCave());
        }
    }



    // Вызывается порталом после боя на арене
    public void OnBattleComplete()
    {
        if (isLoadingScene)
        {
            Debug.Log("⚠ Переход уже выполняется");
            return;
        }


        // В начале каждого цикла всегда начинаем с первой пещеры
        currentCave = 0;


        Debug.Log(
            $"⚔ Бой окончен. Цикл {currentCycle}. " +
            $"currentCave={currentCave}. Переход Cave1"
        );


        LoadScene(caveScenes[currentCave]);
    }



    // Вызывается GateTrigger после выхода из пещеры
    public void LoadNextCave()
    {
        if (isLoadingScene)
            return;


        Debug.Log(
            $"🚪 Выход из пещеры. Цикл {currentCycle}. Текущая пещера {currentCave}"
        );


        // Есть следующая пещера
        if (currentCave < caveScenes.Length - 1)
        {
            currentCave++;

            Debug.Log(
                $"🏔 Переход в пещеру {currentCave + 1}"
            );

            LoadScene(caveScenes[currentCave]);
            return;
        }



        // Завершена третья пещера
        if (currentCycle >= 3)
        {
            Debug.Log("🏆 3 цикл завершен. Финальная сцена");

            GoToFinalScene();
            return;
        }



        // Новый цикл
        currentCycle++;
        currentCave = -1;


        Debug.Log(
            $"🔄 Начинается цикл {currentCycle}"
        );


        LoadScene(fightScene);
    }



    // Оставляем для совместимости с другими скриптами
    public void ReturnToKoryArena()
    {
        if (isLoadingScene)
            return;


        currentCave = -1;


        Debug.Log(
            $"🏟 Возврат на арену. Текущий цикл {currentCycle}"
        );


        LoadScene(fightScene);
    }



    public bool IsLastCave()
    {
        return currentCave == caveScenes.Length - 1;
    }



    public bool IsFinalCycle()
    {
        return currentCycle >= 3;
    }



    public void ShowFinalPanel()
    {
        GoToFinalScene();
    }



    public void ReturnToFight()
    {
        LoadScene(fightScene);
    }



    public void GoToFinalScene()
    {
        Debug.Log("🎬 Переход в Final");

        LoadScene(finalScene);
    }



    private void LoadScene(string sceneName)
    {
        isLoadingScene = true;

        Debug.Log($"➡ Загружаем сцену: {sceneName}");

        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }



    public void ResetGame()
    {
        currentCycle = 1;
        currentCave = -1;
        isLoadingScene = false;


        if (BuffScoreManager.Instance != null)
        {
            BuffScoreManager.Instance.ResetScore();
        }


        Debug.Log("🔄 Игра сброшена");
    }



    private IEnumerator PlacePlayerInCave()
    {
        yield return new WaitForSeconds(0.2f);


        GameObject spawn = GameObject.FindWithTag("CaveSpawnPoint");


        if (spawn == null)
        {
            Debug.Log(
                $"СПАВН: Cycle={currentCycle}, CaveIndex={currentCave}, Scene={SceneManager.GetActiveScene().name}"
            );

            yield break;
        }


        if (PlayerSpawnManager.Instance != null)
        {
            PlayerSpawnManager.Instance.SpawnPlayer(
                spawn.transform.position,
                false
            );


            Debug.Log(
                $"✅ Игрок создан в пещере {currentCave + 1}"
            );
        }
    }



    private IEnumerator ActivateArenaAndPlacePlayer()
    {
        yield return new WaitForSeconds(0.2f);


        if (PlayerSpawnManager.Instance != null)
        {
            PlayerSpawnManager.Instance.SpawnPlayer(
                Vector3.zero,
                true
            );


            Debug.Log(
                $"✅ Игрок создан на арене. Цикл {currentCycle}"
            );
        }
    }



    public string GetProgressInfo()
    {
        return
            $"Цикл {currentCycle}/3, Пещера {currentCave + 1}/{caveScenes.Length}";
    }
}