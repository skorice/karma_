using UnityEngine;

public class BuffScoreManager : MonoBehaviour
{
    public static BuffScoreManager Instance;

    private int score;
    public int Score => score;

    public delegate void ScoreChanged(int newScore);
    public event ScoreChanged OnScoreChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddPoint()
    {
        score = Mathf.Clamp(score + 1, 0, 4);
        Debug.Log("Очки баффа: " + score);
        OnScoreChanged?.Invoke(score);
    }

    public void RemovePoint()
    {
        score = Mathf.Clamp(score - 1, 0, 4);
        Debug.Log("Очки баффа: " + score);
        OnScoreChanged?.Invoke(score);
    }

    public void ResetScore()
    {
        score = 0;
        OnScoreChanged?.Invoke(score);
    }
}
