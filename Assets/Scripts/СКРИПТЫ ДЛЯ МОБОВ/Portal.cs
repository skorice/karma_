using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "Anomaly";

    private bool used;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (used)
            return;

        if (!other.CompareTag("Player"))
            return;


        used = true;

        Debug.Log(" Игрок вошёл в портал! Переход в Anomaly...");

        SceneManager.LoadScene(nextSceneName);
    }
}