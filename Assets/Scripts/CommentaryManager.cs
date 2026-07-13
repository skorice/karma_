using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CommentaryManager : MonoBehaviour
{
    public static CommentaryManager Instance;

    [SerializeField] private TextMeshProUGUI textUI;

    // Хранит ID фраз, которые уже были
    private HashSet<string> triggeredOnce = new HashSet<string>();

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

    // Показ фразы один раз
    public void SayOnce(string id, string text)
    {
        if (triggeredOnce.Contains(id))
            return;

        triggeredOnce.Add(id);
        Show(text);
    }

    // Рандомная фраза один раз
    public void SayRandomOnce(string id, string[] texts)
    {
        if (triggeredOnce.Contains(id))
            return;

        triggeredOnce.Add(id);
        Show(texts[Random.Range(0, texts.Length)]);
    }

    // Показ текста
    public void Show(string text)
    {
        if (textUI == null)
            return;

        textUI.text = text;
    }
}
