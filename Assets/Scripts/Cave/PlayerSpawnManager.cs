using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    public static PlayerSpawnManager Instance;

    [Header("Player Prefab")]
    [SerializeField] private GameObject playerPrefab;

    private GameObject playerInstance;

    [Header("Arena Spawn")]
    [SerializeField] public Transform arenaSpawnPoint;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            SpawnPlayerOnce();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Создаём игрока один раз
    private void SpawnPlayerOnce()
    {
        if (playerInstance != null)
            return;

        if (playerPrefab == null)
        {
            Debug.LogError("PlayerPrefab не назначен в PlayerSpawnManager!");
            return;
        }

        playerInstance = Instantiate(playerPrefab);
        playerInstance.tag = "Player";
        DontDestroyOnLoad(playerInstance);

        Debug.Log("Player создан один раз из префаба.");
    }

    // Обновляем точку спавна арены
    public void RefreshArenaSpawnPoint()
    {
        GameObject obj = GameObject.FindWithTag("ArenaSpawnPoint");

        if (obj == null)
        {
            Debug.LogError("ArenaSpawnPoint не найден в сцене! Убедись, что он есть и имеет тег.");
            return;
        }

        arenaSpawnPoint = obj.transform;
        Debug.Log("ArenaSpawnPoint обновлён.");
    }

    // Перемещаем игрока в нужную точку
    public void MovePlayerTo(Transform point)
    {
        if (point == null)
        {
            Debug.LogError("MovePlayerTo получил пустую точку!");
            return;
        }

        if (playerInstance == null)
        {
            Debug.LogError("PlayerInstance отсутствует! Игрок не создан.");
            return;
        }

        playerInstance.transform.position = point.position;
        Debug.Log($"Игрок перемещён в точку: {point.position}");
    }
}
