using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    public static PlayerSpawnManager Instance;

    [Header("Player Prefab")]
    [SerializeField] private GameObject playerPrefab;
    
    // [Header("Arena Spawn")]
    // [SerializeField] public Transform arenaSpawnPoint;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // SpawnPlayerOnce();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void DestroyPlayer()
    {
        if (player != null)
        {
            Destroy(player);
        }
    }
    
    private GameObject player;
    // Создаём игрока один раз
    public void SpawnPlayer(Vector3 position, bool enableVfx)
    {
        if (playerPrefab == null)
        {
            Debug.LogError("PlayerPrefab не назначен в PlayerSpawnManager!");
            return;
        }

        DestroyPlayer();
        
        player = Instantiate(playerPrefab, position, Quaternion.identity);
        player.tag = "Player";

        if (enableVfx)
        {
            player.GetComponentInChildren<DamageRadiusVfx>().BuildEffect(player.GetComponent<PlayerSettings>().AttackRange);
        }
        
        var camera = GameObject.FindGameObjectWithTag("MainCamera");
        if(camera != null)
        {
            if (camera.TryGetComponent<CameraFollow>(out var cameraFollow))
            {
                cameraFollow.target = player.transform;
            }
        }
    }
}
