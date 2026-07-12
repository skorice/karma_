using UnityEngine;

public class GateTrigger : MonoBehaviour
{
    [SerializeField] private CaveState caveState;
    [SerializeField] private bool isLeftGate;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        CheckChoice();
    }

    private void CheckChoice()
    {
        bool hasAnomaly = caveState.HasAnomaly;

        if (isLeftGate)
        {
            if (hasAnomaly)
                BuffScoreManager.Instance.AddPoint();
            else
                BuffScoreManager.Instance.RemovePoint();
        }
        else
        {
            if (hasAnomaly)
                BuffScoreManager.Instance.RemovePoint();
            else
                BuffScoreManager.Instance.AddPoint();
        }

        // Если это последняя пещера → возвращаемся в бой
        if (CaveManager.Instance.IsLastCave())
        {
            CaveManager.Instance.ReturnToFight();
        }
        else
        {
            CaveManager.Instance.LoadNextCave();
        }
    }
}
