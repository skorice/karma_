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

        // Логика начисления очков — НЕ трогаем
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

        //  ДОПОЛНЕНИЕ: логика конца цикла 
        if (CaveManager.Instance.IsLastCave())
        {
            // Если это последний цикл (3) → показываем финальную панель
            if (CaveManager.Instance.IsFinalCycle())
            {
                CaveManager.Instance.ShowFinalPanel();
            }
            else
            {
                // Иначе возвращаемся на арену
                CaveManager.Instance.ReturnToFight();
            }
        }
        else
        {
            // Если это НЕ последняя пещера → загружаем следующую
            CaveManager.Instance.LoadNextCave();
        }
    }
}
