using UnityEngine;

public class PlayerSpawnManagerLoader : MonoBehaviour
{
    [SerializeField] private GameObject playerSpawnManagerPrefab;

    private void Awake()
    {
        Instantiate(playerSpawnManagerPrefab);
    }
}
