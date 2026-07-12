using UnityEngine;

public class Portal : MonoBehaviour
{
    private bool used;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (used)
            return;

        if (!other.CompareTag("Player"))
            return;

        used = true;

        Debug.Log("Игрок вошёл в портал!");

        CaveManager.Instance.OnBattleComplete();
    }
}