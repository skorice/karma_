using UnityEngine;
using System.Collections;

public class PlayerSpawnManager : MonoBehaviour
{
    [SerializeField] private Transform spawnPointPrefab; // Префаб точки спавна

    private void Start()
    {
        StartCoroutine(DelayedSpawn());
    }

    private IEnumerator DelayedSpawn()
    {
        yield return null; // ждём один кадр, пока игрок появится в сцене
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Player not found! Make sure the player has tag 'Player'.");
            return;
        }

        // Создаём точку спавна из префаба
        Transform spawnPoint = Instantiate(spawnPointPrefab);

        // Перемещаем игрока
        player.transform.position = spawnPoint.position;

        Debug.Log($"Игрок перемещён в точку спавна: {spawnPoint.position}");
    }
}
