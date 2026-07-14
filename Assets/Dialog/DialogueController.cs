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
    
    [Header("Auto Advance Settings")]
    public bool autoAdvance = true; // Включить/выключить автоматическое перелистывание
    public float delayBetweenLines = 2.0f; // Сколько секунд ждать после завершения печати текста

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
    private bool isDialogPlaying = false; 
    
    private Coroutine currentDialogCoroutine; // Ссылка на текущую корутину для безопасной остановки

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
        if (isDialogPlaying || dialoguePanel.activeSelf)
        {
            Debug.Log($"Диалог уже активен для сцены {scene.name}, пропускаем");
            return;
        }
        
        PlayDialogForCurrentScene();
    }

    public void StartNewCycle() 
    {
        int cycle = currentCycle;
        Debug.Log($"=== ДИАЛОГ ЦИКЛА {cycle} ===");
        OnCycleChanged?.Invoke(cycle);
        PlayDialogForCurrentScene();
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
                new DialogLine("Вишну", "Надеюсь, в этот раз она проживёт дольше пяти минут.")
            }
        };

        // СЦЕНА KORYARENA 
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

        // СЦЕНА CAVE1 
        allDialogs["Cave1"] = new DialogScene
        {
            sceneName = "Cave1",
            introLine = new DialogLine("", "Направо - истина, налево - ложь. Сделай свой выбор."),
            randomLines = new DialogLine[]
            {
                new DialogLine("", "Выбор всегда сложен, когда оба пути кажутся верными."),
                new DialogLine("", "Тени на стенах танцуют, но ни одна не укажет путь."),
                new DialogLine("", "Тишина в пещере звенит, как предупреждение."),
                new DialogLine("", "Каждый шаг здесь - проверка на прочность."),
                new DialogLine("", "Свет едва пробивается сквозь тьму."),
                new DialogLine("", "Твой инстинкт - единственный компас здесь."),
                new DialogLine("", "Старые камни хранят тайны, но молчат."),
                new DialogLine("", "Ветер доносит эхо чужих ошибок.")
            },
            variants = new DialogVariant[]
            {
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Шива", "Вот это место мне никогда не нравилось."),
                    new DialogLine("Брахма", "Потому что здесь нельзя ничего разрушить."),
                    new DialogLine("Шива", "Потому что здесь приходится смотреть.")
                }, 1),
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Вишну", "Кажется..."),
                    new DialogLine("Шива", "Не подсказывай.")
                }, 1),
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Брахма", "Ты заметил?"),
                    new DialogLine("Шива", "Что именно?"),
                    new DialogLine("Брахма", "Он посмотрел наверх."),
                    new DialogLine("Вишну", "Все иногда смотрят наверх.")
                }, 1),
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Шива", "Он снова здесь. Как же это предсказуемо."),
                    new DialogLine("Брахма", "Тише. Может, в этот раз он выберет верный путь."),
                    new DialogLine("Вишну", "Мы уже видели этот танец. Снова и снова.")
                }, 2),
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Вишну", "Он колеблется."),
                    new DialogLine("Шива", "Все колеблются перед выбором."),
                    new DialogLine("Брахма", "Но выбор уже сделан. Вопрос лишь в том, осознает ли он это.")
                }, 2),
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Шива", "Последний танец."),
                    new DialogLine("Брахма", "Или первый шаг к чему-то новому?"),
                    new DialogLine("Вишну", "Он уже почти здесь. Ещё одно испытание.")
                }, 3),
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Брахма", "Ты чувствуешь?"),
                    new DialogLine("Шива", "Что именно?"),
                    new DialogLine("Брахма", "Воздух изменился. Он близок к истине.")
                }, 3)
            }
        };

        // СЦЕНА CAVE2 
        allDialogs["Cave2"] = new DialogScene
        {
            sceneName = "Cave2",
            introLine = new DialogLine("", "Ты уже почти у цели. Направо - истина, налево - ложь."),
            randomLines = new DialogLine[]
            {
                new DialogLine("", "Стены здесь ближе, а дышать тяжелее."),
                new DialogLine("", "Кажется, кто-то прошёл здесь до тебя."),
                new DialogLine("", "Выбор становится сложнее с каждым шагом."),
                new DialogLine("", "Тени стали длиннее, а страх - ближе."),
                new DialogLine("", "Здесь каждый камень - свидетель."),
                new DialogLine("", "Ты чувствуешь, как время замедляется."),
                new DialogLine("", "Путь сужается, но выбор остаётся."),
                new DialogLine("", "Твои шаги - единственный звук в этой тишине.")
            },
            variants = new DialogVariant[]
            {
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Шива", "Эти стены помнят каждого, кто здесь проходил."),
                    new DialogLine("Брахма", "И каждый выбирал свой путь."),
                    new DialogLine("Вишну", "Интересно, что выберет он?")
                }, 1),
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Брахма", "Второй коридор. Как быстро летит время."),
                    new DialogLine("Шива", "Для нас время течёт иначе."),
                    new DialogLine("Вишну", "А для него каждая секунда может быть последней.")
                }, 1),
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Вишну", "Он уже не тот, что был в начале."),
                    new DialogLine("Шива", "Смерть меняет всех."),
                    new DialogLine("Брахма", "Но он ещё не умер. Он только учится.")
                }, 1),
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Вишну", "Он запоминает."),
                    new DialogLine("Шива", "Это хорошо или плохо?"),
                    new DialogLine("Брахма", "Это просто значит, что он учится. А учиться - значит меняться.")
                }, 2),
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Шива", "Снова этот коридор."),
                    new DialogLine("Брахма", "Но он уже не тот, что был раньше."),
                    new DialogLine("Вишну", "Или это мы изменились?")
                }, 2),
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Брахма", "Он начинает понимать закономерности."),
                    new DialogLine("Шива", "Или ему просто везёт?"),
                    new DialogLine("Вишну", "Везение - это тоже навык.")
                }, 2),
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Брахма", "Он почти научился слушать."),
                    new DialogLine("Шива", "Слушать - не значит слышать."),
                    new DialogLine("Вишну", "Но он слышит. Я вижу это по его глазам.")
                }, 3),
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Шива", "Ещё один шаг."),
                    new DialogLine("Брахма", "Или пропасть?"),
                    new DialogLine("Вишну", "Иногда это одно и то же.")
                }, 3),
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Вишну", "Он приближается к разгадке."),
                    new DialogLine("Шива", "К какой именно?"),
                    new DialogLine("Брахма", "К той, что ждёт его в конце пути.")
                }, 3)
            }
        };

        // СЦЕНА CAVE3 
        allDialogs["Cave3"] = new DialogScene
        {
            sceneName = "Cave3",
            introLine = new DialogLine("", "Последний выбор. Направо - истина, налево - ложь."),
            randomLines = new DialogLine[]
            {
                new DialogLine("", "Ты почти у цели. Чувствуешь?"),
                new DialogLine("", "Свет в конце тоннеля - это выход или ловушка?"),
                new DialogLine("", "Последние шаги - самые трудные."),
                new DialogLine("", "Здесь воздух пахнет свободой."),
                new DialogLine("", "Ты прошёл долгий путь, чтобы оказаться здесь."),
                new DialogLine("", "Что ты выберешь, когда выбора почти не осталось?"),
                new DialogLine("", "Финал близок. Или это только начало?"),
                new DialogLine("", "Твоя судьба - в твоих руках.")
            },
            variants = new DialogVariant[]
            {
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Брахма", "Последняя пещера."),
                    new DialogLine("Шива", "Или предпоследняя?"),
                    new DialogLine("Вишну", "Для него - последняя. Дальше будет только финал.")
                }, 1),
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Шива", "Третий раз. Третий коридор."),
                    new DialogLine("Брахма", "Третий шанс."),
                    new DialogLine("Вишну", "Или третья ловушка.")
                }, 1),
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Вишну", "Он почти у цели."),
                    new DialogLine("Шива", "Но цель - это всегда начало."),
                    new DialogLine("Брахма", "Посмотрим, готов ли он к тому, что ждёт впереди.")
                }, 1),
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Вишну", "Он начинает понимать правила."),
                    new DialogLine("Шива", "Какие именно?"),
                    new DialogLine("Вишну", "Не этой игры.")
                }, 2),
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Шива", "Сколько раз можно проходить одно и то же?"),
                    new DialogLine("Брахма", "Столько, сколько потребуется."),
                    new DialogLine("Вишну", "Главное - сделать правильный вывод.")
                }, 2),
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Брахма", "Он чувствует приближение конца."),
                    new DialogLine("Шива", "Или начала?"),
                    new DialogLine("Вишну", "Всё зависит от того, как на это посмотреть.")
                }, 2),
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Брахма", "Я вижу свет в его глазах."),
                    new DialogLine("Шива", "Это свет истины или просто отражение?"),
                    new DialogLine("Вишну", "Разница не так уж велика, когда ты стоишь на пороге.")
                }, 3),
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Шива", "Он готов."),
                    new DialogLine("Брахма", "Ты уверен?"),
                    new DialogLine("Шива", "Нет. Но это и есть самое интересное.")
                }, 3),
                new DialogVariant(new DialogLine[]
                {
                    new DialogLine("Вишну", "Время вышло."),
                    new DialogLine("Брахма", "Или только начинается?"),
                    new DialogLine("Шива", "Для него - начинается. Для нас - заканчивается.")
                }, 3)
            }
        };

        // СЦЕНА FINAL 
        allDialogs["Final"] = new DialogScene
        {
            sceneName = "Final",
            lines = new DialogLine[]
            {
                new DialogLine("", "Допустим. Ты уверен, что всё-таки вышел из игры?"),
                new DialogLine("", "Тогда ответь на один вопрос..."),
                new DialogLine("", "Что было в начале?"),
                new DialogLine("", "И что будет в конце?")
            }
        };
    }

    void PlayDialogForCurrentScene()
    {
        if (isDialogPlaying || dialoguePanel.activeSelf)
        {
            Debug.Log("Диалог уже играет, пропускаем");
            return;
        }

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
        isDialogPlaying = true;
        yield return new WaitForSeconds(0.3f);

        dialoguePanel.SetActive(true);
        currentLines.Clear();
        dialogFinished = false;

        if (currentCycle == 1)
        {
            if (scene.introLine != null && !string.IsNullOrEmpty(scene.introLine.text))
            {
                currentLines.Enqueue(scene.introLine);
            }
        }
        else
        {
            if (scene.randomLines != null && scene.randomLines.Length > 0)
            {
                int randomIndex = Random.Range(0, scene.randomLines.Length);
                DialogLine randomLine = scene.randomLines[randomIndex];
                currentLines.Enqueue(randomLine);
            }
        }

        DialogLine[] linesToUse;

        if (scene.variants != null && scene.variants.Length > 0)
        {
            List<DialogVariant> availableVariants = new List<DialogVariant>();
            foreach (var variant in scene.variants)
            {
                if (variant.cycle == 0 || variant.cycle == currentCycle)
                {
                    availableVariants.Add(variant);
                }
            }

            if (availableVariants.Count > 0)
            {
                int variantIndex = Random.Range(0, availableVariants.Count);
                linesToUse = availableVariants[variantIndex].lines;
            }
            else
            {
                linesToUse = scene.variants[0].lines;
            }
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

        // Оставляем возможность ручного пропуска для тех, кто читает быстро
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                // Если текст еще печатается, прерываем корутину и выводим весь текст
                if (currentDialogCoroutine != null) StopCoroutine(currentDialogCoroutine);
                dialogueText.text = currentFullText;
                isTyping = false;
                
                // Запускаем таймер ожидания до следующей строки
                if (autoAdvance)
                {
                    currentDialogCoroutine = StartCoroutine(WaitAndShowNext());
                }
            }
            else
            {
                // Если текст уже напечатан (идет ожидание), сразу переходим к следующему
                if (currentDialogCoroutine != null) StopCoroutine(currentDialogCoroutine);
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
            isDialogPlaying = false;
            
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
            
        // Останавливаем предыдущую корутину перед запуском новой
        if (currentDialogCoroutine != null) StopCoroutine(currentDialogCoroutine);
        currentDialogCoroutine = StartCoroutine(TypeTextAndAdvance(currentFullText));
    }

    // Новая корутина, которая печатает текст, а затем ждет и переключает на следующий
    IEnumerator TypeTextAndAdvance(string fullText)
    {
        isTyping = true;
        dialogueText.text = "";

        // 1. Печатаем текст
        foreach (char c in fullText)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;

        // 2. Ждем заданное время и автоматически переходим дальше
        if (autoAdvance)
        {
            yield return new WaitForSeconds(delayBetweenLines);
            ShowNextLine();
        }
    }

    // Корутина для ожидания, если игрок пропустил анимацию печати текста
    IEnumerator WaitAndShowNext()
    {
        yield return new WaitForSeconds(delayBetweenLines);
        ShowNextLine();
    }


    // ПУБЛИЧНЫЕ МЕТОДЫ ДЛЯ ВЫЗОВА ИЗ ДРУГИХ СКРИПТОВ 

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
        if (isDialogPlaying || dialoguePanel.activeSelf)
        {
            Debug.Log("Диалог уже играет, пропускаем StartDialogForScene");
            return;
        }

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

    public bool IsDialogFinished()
    {
        return dialogFinished;
    }

    public void ResetGame()
    {
        currentCycle = 0;
        dialogFinished = false;
        isDialogPlaying = false;
        instanceExists = false;
        Destroy(gameObject);
        SceneManager.LoadScene("Start");
    }
}

// КЛАССЫ ДЛЯ ХРАНЕНИЯ ДАННЫХ 

[System.Serializable]
public class DialogScene
{
    public string sceneName;
    public DialogLine[] lines;
    public DialogVariant[] variants;
    public DialogLine introLine;
    public DialogLine[] randomLines;

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
    public int cycle;

    public DialogVariant() { }
    
    public DialogVariant(DialogLine[] lines, int cycle = 0)
    {
        this.lines = lines;
        this.cycle = cycle;
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