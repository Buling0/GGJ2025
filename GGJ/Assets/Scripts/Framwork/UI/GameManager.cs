using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 关卡目标分数数组，StartTest和EndTest的目标分数设置为0
    private int[] levelTargetScores = { 0, 10, 0, 20, 0, 30, 0, 40, 0, 50, 0 };
    private string[] sceneNames = { "StartTest", "Scene1Test", "LevelOk 1", "Scene2Test", "LevelOk 2", "Scene3Test", "LevelOk 3", "Scene4Test", "LevelOk 4", "Scene5Test", "EndTest" };
    private int currentSceneIndex = 0;
    private int currentScore = 0;

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
        // 计算当前关卡的目标分数索引
        int targetScoreIndex = (currentSceneIndex - 1) / 2;
        if (targetScoreIndex >= 0 && targetScoreIndex < levelTargetScores.Length)
        {
            return levelTargetScores[targetScoreIndex];
        }
        return 0;
    }

    public void LoadNextScene()
    {
        currentSceneIndex++;
        if (currentSceneIndex < sceneNames.Length)
        {
            LoadScene(sceneNames[currentSceneIndex]);
        }
        else
        {
            Debug.LogWarning("No more scenes to load.");
        }
    }

    public void LoadNextLevel()
    {
        currentSceneIndex++;
        if (currentSceneIndex < sceneNames.Length)
        {
            LoadScene(sceneNames[currentSceneIndex]);
        }
        else
        {
            Debug.LogWarning("No more levels to load.");
        }
    }

    private void LoadScene(string sceneName)
    {
        Debug.Log($"Loading Scene: {sceneName} (Index: {currentSceneIndex})");
        SceneManager.LoadScene(sceneName);
    }
}