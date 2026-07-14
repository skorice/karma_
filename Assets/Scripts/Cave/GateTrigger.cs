using UnityEngine;

public class GateTrigger : MonoBehaviour
{
    [SerializeField] private bool isExitGate = true;

    private bool isTriggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (!isExitGate)
            return;

        if (isTriggered)
            return;

        isTriggered = true;

        Debug.Log("🚪 Игрок покинул пещеру");

        CaveManager.Instance.LoadNextCave();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isTriggered = false;
    }
}