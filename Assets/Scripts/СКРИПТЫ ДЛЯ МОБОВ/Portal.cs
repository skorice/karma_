using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{

    private bool isActive = true; // Чтобы портал был активен не всегда

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        Debug.Log("🚪 Игрок вошёл в портал! Переход в пещеру...");

        // ИДля перехода
        if (CaveManager.Instance != null)
        {
           // CaveManager.Instance.GoToAnomaly();
        }
        else
        {
            // Если CaveManager нет (например, первый раз), используем SceneManager
            Debug.LogWarning("CaveManager не найден, используем SceneManager");
            SceneManager.LoadScene("Anomaly"); // Укажите название вашей сцены пещеры
        }
    }
}