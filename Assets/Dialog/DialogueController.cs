using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DialogueController : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI dialogueText;
    public GameObject dialoguePanel;
    public float typingSpeed = 0.05f;

    [Header("Cycle Settings")]
    public int maxCycles = 3;
    private int currentCycle = 0;

    private Queue<DialogLine> currentLines;
    private bool isTyping = false;
    private string currentFullText = "";
    private bool dialogFinished = false;

    private Dictionary<string, DialogScene> allDialogs;

    public System.Action<int> OnCycleChanged;

    private static bool instanceExists = false;

    void Awake()
    {
        if (instanceExists)
        {
            Debug.Log("DialogueManager уже существует. Удаляем дубликат.");
            Destroy(gameObject);
            return;
        }

        instanceExists = true;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        currentLines = new Queue<DialogLine>();
        
        InitializeDialogs();
        
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        StartNewCycle();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        
        if (instanceExists)
        {
            instanceExists = false;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayDialogForCurrentScene();
    }

    public void StartNewCycle()
    {
        if (currentCycle < maxCycles)
        {
            currentCycle++;
            Debug.Log($"=== НАЧАЛО ЦИКЛА {currentCycle} ===");
            
            OnCycleChanged?.Invoke(currentCycle);
            
            PlayDialogForCurrentScene();
        }
        else
        {
            Debug.Log("Все 3 цикла завершены!");
            SceneManager.LoadScene("Final");
        }
    }

    void InitializeDialogs()
    {
        allDialogs = new Dictionary<string, DialogScene>();

        // СЦЕНА START 
        allDialogs["Start"] = new DialogScene
        {
            sceneName = "Start",
            lines = new DialogLine[]
            {
                new DialogLine("Шива", "Твоя очередь."),
                new DialogLine("Брахма", "По правилам сначала выбирается фигура."),
                new DialogLine("Вишну", "Надеюсь, в этот раз она проживёт дольше пяти минут."),
                new DialogLine("Брахма", "Новый цикл, но правила прежние. Выживешь здесь - сможешь пройти дальше.")
            }
        };

        //  СЦЕНА KORYARENA 
        allDialogs["KoryArena"] = new DialogScene
        {
            sceneName = "KoryArena",
            variants = new DialogVariant[]
            {
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Шива", "Я поставил на тебя."),
                    new DialogLine("Вишну", "Не ставь на смертных."),
                    new DialogLine("Шива", "Поэтому и интересно.")
                }),
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Вишну", "А наша фигурка точно жива? Двигается, как будто не очень.")
                }),
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Шива", "Посмотрим, сколько ты продержишься.")
                })
            }
        };

        //  СЦЕНА CAVE1 
        allDialogs["Cave1"] = new DialogScene
        {
            sceneName = "Cave1",
            variants = new DialogVariant[]
            {
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Шива", "Вот это место мне никогда не нравилось."),
                    new DialogLine("Брахма", "Потому что здесь нельзя ничего разрушить."),
                    new DialogLine("Шива", "Потому что здесь приходится смотреть.")
                }),
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Вишну", "Кажется..."),
                    new DialogLine("Шива", "Не подсказывай.")
                }),
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Брахма", "Ты заметил?"),
                    new DialogLine("Шива", "Что именно?"),
                    new DialogLine("Брахма", "Он посмотрел наверх."),
                    new DialogLine("Вишну", "Все иногда смотрят наверх.")
                }),
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Вишну", "Кажется, он начинает понимать правила."),
                    new DialogLine("Шива", "Какие именно?"),
                    new DialogLine("Вишну", "Не этой игры.")
                })
            }
        };

        //  СЦЕНА CAVE2 
        allDialogs["Cave2"] = new DialogScene
        {
            sceneName = "Cave2",
            variants = new DialogVariant[]
            {
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Шива", "Вот это место мне никогда не нравилось."),
                    new DialogLine("Брахма", "Потому что здесь нельзя ничего разрушить."),
                    new DialogLine("Шива", "Потому что здесь приходится смотреть.")
                }),
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Вишну", "Кажется..."),
                    new DialogLine("Шива", "Не подсказывай.")
                }),
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Брахма", "Ты заметил?"),
                    new DialogLine("Шива", "Что именно?"),
                    new DialogLine("Брахма", "Он посмотрел наверх."),
                    new DialogLine("Вишну", "Все иногда смотрят наверх.")
                }),
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Вишну", "Кажется, он начинает понимать правила."),
                    new DialogLine("Шива", "Какие именно?"),
                    new DialogLine("Вишну", "Не этой игры.")
                })
            }
        };

        //  СЦЕНА CAVE3 
        allDialogs["Cave3"] = new DialogScene
        {
            sceneName = "Cave3",
            variants = new DialogVariant[]
            {
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Шива", "Вот это место мне никогда не нравилось."),
                    new DialogLine("Брахма", "Потому что здесь нельзя ничего разрушить."),
                    new DialogLine("Шива", "Потому что здесь приходится смотреть.")
                }),
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Вишну", "Кажется..."),
                    new DialogLine("Шива", "Не подсказывай.")
                }),
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Брахма", "Ты заметил?"),
                    new DialogLine("Шива", "Что именно?"),
                    new DialogLine("Брахма", "Он посмотрел наверх."),
                    new DialogLine("Вишну", "Все иногда смотрят наверх.")
                }),
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Вишну", "Кажется, он начинает понимать правила."),
                    new DialogLine("Шива", "Какие именно?"),
                    new DialogLine("Вишну", "Не этой игры.")
                })
            }
        };

        // СЦЕНА FINAL 
        allDialogs["Final"] = new DialogScene
        {
            sceneName = "Final",
            lines = new DialogLine[]
            {
                new DialogLine("", "Допустим. Ты уверен, что все-таки вышел из игры?")
            }
        };
    }

    void PlayDialogForCurrentScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        
        if (sceneName == "Final")
        {
            if (allDialogs.ContainsKey(sceneName))
            {
                StartCoroutine(PlayDialogScene(allDialogs[sceneName]));
            }
            return;
        }

        if (!allDialogs.ContainsKey(sceneName))
        {
            Debug.LogWarning($"Нет диалога для сцены: {sceneName}");
            dialoguePanel.SetActive(false);
            return;
        }

        StartCoroutine(PlayDialogScene(allDialogs[sceneName]));
    }

    IEnumerator PlayDialogScene(DialogScene scene)
    {
        yield return new WaitForSeconds(0.3f);

        dialoguePanel.SetActive(true);
        currentLines.Clear();
        dialogFinished = false;

        DialogLine[] linesToUse;

        if (scene.variants != null && scene.variants.Length > 0)
        {
            int variantIndex = Random.Range(0, scene.variants.Length);
            linesToUse = scene.variants[variantIndex].lines;
            Debug.Log($"Сцена {scene.sceneName}, Цикл {currentCycle}: выбран вариант {variantIndex + 1}");
        }
        else
        {
            linesToUse = scene.lines;
        }

        foreach (var line in linesToUse)
        {
            currentLines.Enqueue(line);
        }

        ShowNextLine();
    }

    void Update()
    {
        if (dialogFinished) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogueText.text = currentFullText;
                isTyping = false;
            }
            else
            {
                ShowNextLine();
            }
        }
    }

    void ShowNextLine()
    {
        if (currentLines.Count == 0)
        {
            dialoguePanel.SetActive(false);
            dialogFinished = true;
            
            string currentScene = SceneManager.GetActiveScene().name;
            if (currentScene != "Final")
            {
                Debug.Log($"Цикл {currentCycle} завершён в сцене {currentScene}");
            }
            return;
        }

        DialogLine line = currentLines.Dequeue();
        
        if (!string.IsNullOrEmpty(line.speaker))
            currentFullText = $"{line.speaker}: {line.text}";
        else
            currentFullText = line.text;
            
        StartCoroutine(TypeText(currentFullText));
    }

    IEnumerator TypeText(string fullText)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in fullText)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    //  ПУБЛИЧНЫЕ МЕТОДЫ ДЛЯ ВЫЗОВА ИЗ ДРУГИХ СКРИПТОВ 

    public void PlayerDied()
    {
        if (currentCycle < maxCycles)
        {
            Debug.Log($"Игрок умер. Запускаем цикл {currentCycle + 1}");
            StartNewCycle();
        }
        else
        {
            Debug.Log("Все циклы закончены. Игра окончена.");
            SceneManager.LoadScene("Final");
        }
    }

    public void NextLevel()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        
        switch (currentScene)
        {
            case "Start":
                SceneManager.LoadScene("KoryArena");
                break;
            case "KoryArena":
                SceneManager.LoadScene("Cave1");
                break;
            case "Cave1":
                SceneManager.LoadScene("Cave2");
                break;
            case "Cave2":
                SceneManager.LoadScene("Cave3");
                break;
            case "Cave3":
                SceneManager.LoadScene("Final");
                break;
            default:
                Debug.LogWarning("Неизвестная сцена: " + currentScene);
                break;
        }
    }

    public void StartDialogForScene(string sceneName)
    {
        if (allDialogs.ContainsKey(sceneName))
        {
            StartCoroutine(PlayDialogScene(allDialogs[sceneName]));
        }
        else
        {
            Debug.LogWarning($"Диалог для сцены {sceneName} не найден");
        }
    }

    public int GetCurrentCycle()
    {
        return currentCycle;
    }

    public void ResetGame()
    {
        currentCycle = 0;
        dialogFinished = false;
        instanceExists = false;
        Destroy(gameObject);
        SceneManager.LoadScene("Start");
    }
}

//  КЛАССЫ ДЛЯ ХРАНЕНИЯ ДАННЫХ 

[System.Serializable]
public class DialogScene
{
    public string sceneName;
    public DialogLine[] lines;
    public DialogVariant[] variants;

    public DialogScene() { }
    
    public DialogScene(string name, DialogLine[] lines)
    {
        this.sceneName = name;
        this.lines = lines;
    }
}

[System.Serializable]
public class DialogVariant
{
    public DialogLine[] lines;

    public DialogVariant() { }
    
    public DialogVariant(DialogLine[] lines)
    {
        this.lines = lines;
    }
}

[System.Serializable]
public class DialogLine
{
    public string speaker;
    public string text;

    public DialogLine() { }
    
    public DialogLine(string speaker, string text)
    {
        this.speaker = speaker;
        this.text = text;
    }
}