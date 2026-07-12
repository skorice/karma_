using UnityEngine;

public class CaveState : MonoBehaviour
{
    public static CaveState Instance { get; private set; }

    [SerializeField] private bool hasAnomaly;

    public bool HasAnomaly => hasAnomaly;

    private void Awake()
    {
        Instance = this;
    }

    public void SetAnomalyState(bool value)
    {
        hasAnomaly = value;
    }
}
