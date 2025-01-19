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
    private int[] levelTargetScores = { 0, 10, 0, 20, 0, 30, 0, 40, 0, 50, 0 };
    private string[] sceneNames = { "StartTest", "Scene1Test", "LevelOk 1", "Scene2Test", "LevelOk 2", "Scene3Test", "LevelOk 3", "Scene4Test", "LevelOk 4", "Scene5Test", "EndTest" };
    private int currentSceneIndex = 0;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>(); // 获取GameManager的引用
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
        currentSceneIndex = (currentSceneIndex + 1) % sceneNames.Length; // 增加场景索引
        Debug.Log("Loading Scene: " + sceneNames[currentSceneIndex]); // 输出调试信息
        SceneManager.LoadScene(sceneNames[currentSceneIndex]); // 加载下一个场景
    }

    // 根据当前场景设置目标分数
    private void SetTargetScoreBasedOnScene()
    {
        string sceneName = SceneManager.GetActiveScene().name; // 获取当前场景名称

        // 根据场景名称设置目标分数
        switch (sceneName)
        {
            case "StartTest":
                SetTargetScore(levelTargetScores[0]);
                break;
            case "Scene1Test":
                SetTargetScore(levelTargetScores[1]);
                break;
            case "Scene2Test":
                SetTargetScore(levelTargetScores[3]);
                break;
            case "Scene3Test":
                SetTargetScore(levelTargetScores[5]);
                break;
            case "Scene4Test":
                SetTargetScore(levelTargetScores[7]);
                break;
            case "Scene5Test":
                SetTargetScore(levelTargetScores[9]);
                break;
            case "LevelOk 1":
            case "LevelOk 2":
            case "LevelOk 3":
            case "LevelOk 4":
                SetTargetScore(0); // LevelOK 场景不需要目标分数
                break;
            case "EndTest":
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
        EventManager.GetInstance().AddEventListener<int>("ScoreChange", RefreshScore);
    }

    private void UnRegister()
    {
        EventManager.GetInstance().RemoveEventListener<int>("ScoreChange", RefreshScore);
    }

    private void OnDestroy()
    {
        UnRegister(); // 注销事件监听器
    }

    private void RefreshScore(int score)
    {
        currentScore = score; // 更新当前分数
        currentScoreValueText.text = currentScore.ToString(); // 更新UI显示
    }
} 