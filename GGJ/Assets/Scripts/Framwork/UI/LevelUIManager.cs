using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelUIManager : MonoBehaviour
{
    public Text targetScoreLabelText; // 显示关卡目标分数的标签
    public Text targetScoreValueText; // 显示目标分数的数值
    public Text currentScoreLabelText; // 显示当前分数的标签
    public Text currentScoreValueText; // 显示当前分数的数值
    public Text timerText; // 显示计时器

    private int targetScore; // 当前关卡的目标分数
    private int currentScore; // 当前玩家的分数
    private float timer; // 计时器
    private float logTimer = 0f; // 用于控制调试信息输出的计时器

    private GameManager gameManager; // 引用GameManager以便更新分数和切换场景

    // 各个场景的目标分数，StartTest和EndTest的目标分数为0
    private int[] levelTargetScores = { 0, 128, 256, 512, 1024, 2048, 0 };
    private string[] sceneNames = { "StartTest", "Scene1Test", "Scene2Test", "Scene3Test", "Scene4Test", "Scene5Test", "EndTest" };
    private int currentSceneIndex = 0; // 当前场景的索引

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>(); // 获取GameManager的引用
        InitializeCurrentSceneIndex(); // 初始化当前场景索引

        SetTargetScoreBasedOnScene(); // 根据当前场景设置目标分数
        currentScore = 0; // 初始化当前分数
        UpdateScoreUI(); // 更新分数显示
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

        // 在Scene1Test到Scene5Test中每30秒输出一次关卡目标分数和当前玩家的分数
        if (currentSceneIndex >= 1 && currentSceneIndex <= 5 && logTimer >= 3f)
        {
            Debug.Log($"Current Score: {currentScore}, Target Score: {targetScore}");
            logTimer = 0f; // 重置计时器
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
                SetTargetScore(levelTargetScores[2]);
                break;
            case "Scene3Test":
                SetTargetScore(levelTargetScores[3]);
                break;
            case "Scene4Test":
                SetTargetScore(levelTargetScores[4]);
                break;
            case "Scene5Test":
                SetTargetScore(levelTargetScores[5]);
                break;
            case "EndTest":
                SetTargetScore(levelTargetScores[6]);
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
        targetScoreLabelText.text = "关卡目标分数";
        targetScoreValueText.text = targetScore.ToString();
    }

    // 增加分数并更新UI
    public void AddScore(int scoreToAdd)
    {
        currentScore += scoreToAdd;
        UpdateScoreUI();
        gameManager.AddScore(scoreToAdd); // 通知GameManager更新分数
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
} 