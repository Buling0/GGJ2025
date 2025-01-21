using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // 添加TextMeshPro的命名空间

public class LevelUIManager : MonoBehaviour
{
    public Text targetScoreLabelText; // 显示关卡目标分数的标签
    public Text targetScoreValueText; // 显示目标分数的数值
    public Text currentScoreLabelText; // 显示当前分数的标签
    public Text currentScoreValueText; // 显示当前分数的数值
    public Text timerText; // 显示计时器
    public TextMeshProUGUI tmp; // 添加TextMeshProUGUI的引用

    private int targetScore; // 当前关卡的目标分数
    private int currentScore; // 当前玩家的分数
    private float timer; // 计时器
    private float logTimer = 0f; // 用于控制调试信息输出的计时器

    private GameManager gameManager; // 引用GameManager以便更新分数和切换场景

    // 各个场景的目标分数，StartTest和EndTest的目标分数为0
        private int[] levelTargetScores = { 0, 10, 10, 20, 20, 30, 30, 40, 40, 50, 0 };
    private string[] sceneNames = { 
        "Start", 
        "Level1",
        "LevelWin", 
        "Level2",
        "LevelWin",  
        "Level3", 
        "LevelWin", 
        "Level4", 
        "LevelWin", 
        "Level5", 
        "End" 
        };
    private int currentSceneIndex = 0;

    void Start()
    {
        gameManager = GameManager.Instance; // 使用单例访问
        InitializeCurrentSceneIndex(); // 初始化当前场景索引

        SetTargetScoreBasedOnScene(); // 根据当前场景设置目标分数
        currentScore = 0; // 初始化当前分数
        UpdateScoreUI(); // 更新分数显示

        Register(); // 注册事件监听器
    }

    void Update()
    {
        timer += Time.deltaTime; // 更新计时器
        logTimer += Time.deltaTime; // 更新调试信息计时器
        UpdateTimerUI(); // 更新计时器显示

        // 检测按键"P"并切换到下一个场景
        if (Input.GetKeyDown(KeyCode.P))
        {
            LoadNextScene();
        }

        // 在Scene1Test到Scene5Test中每1秒输出一次关卡目标分数和当前玩家的分数
        if (currentSceneIndex >= 1 && currentSceneIndex <= 10 && logTimer >= 1f)
        {
            Debug.Log($"Current Score: {currentScore}, Target Score: {targetScore}");
            logTimer = 0f; // 重置计时器
        }

        // 检查当前分数是否达到目标分数
        if (currentScore >= targetScore)
        {
            LoadNextScene();
        }
    }

    // 初始化当前场景索引
    private void InitializeCurrentSceneIndex()
    {
        string currentSceneName = SceneManager.GetActiveScene().name; // 获取当前场景名称

        // 找到当前场景在sceneNames数组中的索引
        for (int i = 0; i < sceneNames.Length; i++)
        {
            if (sceneNames[i] == currentSceneName)
            {
                currentSceneIndex = i;
                break;
            }
        }
    }

    // 加载下一个场景
    private void LoadNextScene()
    {
        GameManager.Instance.LoadNextLevel();
    }

    // 根据当前场景设置目标分数
    private void SetTargetScoreBasedOnScene()
    {
        string sceneName = SceneManager.GetActiveScene().name; // 获取当前场景名称

        // 根据场景名称设置目标分数
        switch (sceneName)
        {
            case "Start":
                SetTargetScore(levelTargetScores[0]);
                break;
            case "Level1":
                SetTargetScore(levelTargetScores[1]);
                break;
            case "Level2":
                SetTargetScore(levelTargetScores[3]);
                break;
            case "Level3":
                SetTargetScore(levelTargetScores[5]);
                break;
            case "Level4":
                SetTargetScore(levelTargetScores[7]);
                break;
            case "Level5":
                SetTargetScore(levelTargetScores[9]);
                break;
            case "LevelWin":
                SetTargetScore(levelTargetScores[2]);
                break;
            case "End":
                SetTargetScore(levelTargetScores[10]);
                break;
            default:
                Debug.LogWarning("未识别的场景名称，无法设置目标分数");
                break;
        }
    }

    // 设置目标分数并更新UI
    public void SetTargetScore(int score)
    {
        targetScore = score;
        targetScoreLabelText.text = "目标分数";
        targetScoreValueText.text = targetScore.ToString();
    }

    // 增加分数并更新UI
    public void AddScore(int scoreToAdd)
    {
        currentScore += scoreToAdd;
        UpdateScoreUI();
        gameManager.AddScore(scoreToAdd); // 通知GameManager更新分数

        // 检查当前分数是否达到目标分数
        if (currentScore >= targetScore)
        {
            LoadNextScene();
        }
    }

    // 更新分数显示
    private void UpdateScoreUI()
    {
        currentScoreLabelText.text = "当前分数";
        currentScoreValueText.text = currentScore.ToString();
    }

    // 更新计时器显示
    private void UpdateTimerUI()
    {
        int hours = (int)(timer / 3600);
        int minutes = (int)((timer % 3600) / 60);
        int seconds = (int)(timer % 60);
        timerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
    }

    private void Register()
    {
        if (EventManager.GetInstance() != null)
        {
            EventManager.GetInstance().AddEventListener<int>("ScoreChange", RefreshScore);
        }
    }

    private void UnRegister()
    {
        if (EventManager.GetInstance() != null)
        {
            EventManager.GetInstance().RemoveEventListener<int>("ScoreChange", RefreshScore);
        }
    }

    private void OnEnable()
    {
        Register();
    }

    private void OnDisable()
    {
        UnRegister();
    }

    private void OnDestroy()
    {
        UnRegister();
    }

    private void RefreshScore(int score)
    {
        currentScore = score; // 更新当前分数
        currentScoreValueText.text = currentScore.ToString(); // 更新UI显示
    }
} 