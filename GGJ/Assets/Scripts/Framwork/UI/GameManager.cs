using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 关卡目标分数数组，StartTest和EndTest的目标分数设置为0
    private int[] levelTargetScores = { 0, 128, 256, 512, 1024, 2048, 0 };
    private string[] sceneNames = { "StartTest", "Scene1Test", "Scene2Test", "Scene3Test", "Scene4Test", "Scene5Test", "EndTest" };
    private int currentSceneIndex = 0;
    private int currentScore = 0;

    private SceneTransitionManager sceneTransitionManager;

    private void Start()
    {
        sceneTransitionManager = FindObjectOfType<SceneTransitionManager>();
    }

    public void StartGame()
    {
        Debug.Log("StartGame called");
        currentScore = 0;
        currentSceneIndex = 1; // 设置为1以跳转到Scene1Test
        LoadScene(sceneNames[currentSceneIndex]);
    }

    public void AddScore(int scoreToAdd)
    {
        currentScore += scoreToAdd;
        Debug.Log($"Current Score: {currentScore}, Target Score: {GetTargetScoreForCurrentScene()}"); // 添加调试信息
        CheckForLevelCompletion();
    }

    private void CheckForLevelCompletion()
    {
        if (currentScore >= GetTargetScoreForCurrentScene())
        {
            Debug.Log("Level completed, loading next scene.");
            LoadNextScene();
        }
        else
        {
            Debug.Log("Level not completed yet.");
        }
    }

    private int GetTargetScoreForCurrentScene()
    {
        if (currentSceneIndex >= 0 && currentSceneIndex < levelTargetScores.Length)
        {
            return levelTargetScores[currentSceneIndex];
        }
        return 0;
    }

    private void LoadNextScene()
    {
        currentSceneIndex++;
        if (currentSceneIndex < sceneNames.Length - 1)
        {
            LoadScene(sceneNames[currentSceneIndex]);
        }
        else
        {
            LoadScene("EndTest");
        }
    }

    private void LoadScene(string sceneName)
    {
        Debug.Log("Loading Scene: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }
}