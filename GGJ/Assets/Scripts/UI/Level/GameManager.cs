using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    
    public static GameManager Instance
    {
        get 
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    instance = go.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }

    private AudioListener persistentAudioListener;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            // 创建持久的 AudioListener
            if (persistentAudioListener == null)
            {
                GameObject audioObj = new GameObject("PersistentAudioListener");
                persistentAudioListener = audioObj.AddComponent<AudioListener>();
                // 先设置 DontDestroyOnLoad，再设置父对象
                DontDestroyOnLoad(audioObj);
                audioObj.transform.SetParent(transform, true);
            }
            
            InitializeGameManager();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitializeGameManager()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        currentSceneIndex = System.Array.IndexOf(sceneNames, currentSceneName);
        if (currentSceneIndex == -1) currentSceneIndex = 0;
        currentScore = 0;
        // 在这里可以添加其他初始化逻辑
    }

    // 关卡目标分数数组，StartTest和EndTest的目标分数设置为0
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
    private int currentScore = 0;

    public void StartGame()
    {
        Debug.Log("StartGame called");
        // 重置游戏状态
        currentScore = 0;
        currentSceneIndex = 1; // 设置为1以跳转到Level1
        
        // 在加载新场景前清理当前场景
        CleanupCurrentScene();
        
        LoadScene(sceneNames[currentSceneIndex]);
    }

    private void CleanupCurrentScene()
    {
        // 停止所有协程
        StopAllCoroutines();
        
        // 只清理特定的事件监听
        if (EventManager.GetInstance() != null)
        {
            EventManager.GetInstance().RemoveEventListener<int>("ScoreChange", null);
        }
        
        // 销毁所有不需要持久化的对象，但保留必要的管理器
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj != gameObject && 
                !obj.CompareTag("DontDestroy") && 
                obj.GetComponent<AudioListener>() != persistentAudioListener &&
                !obj.name.Contains("Manager")) // 保留管理器对象
            {
                Destroy(obj);
            }
        }

        // 不再强制进行资源回收
        // Resources.UnloadUnusedAssets();
        // System.GC.Collect();
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
            LoadNextLevel();
        }
        else
        {
            Debug.Log("Level not completed yet.");
        }
    }

    private int GetTargetScoreForCurrentScene()
    {
        // 计算当前关卡的目标分数索引
        int targetScoreIndex = (currentSceneIndex - 1);
        if (targetScoreIndex >= 0 && targetScoreIndex < levelTargetScores.Length)
        {
            return levelTargetScores[targetScoreIndex];
        }
        return 0;
    }

    public void LoadNextLevel()
    {
        Debug.Log($"Current scene index before increment: {currentSceneIndex}");
        currentSceneIndex++;
        
        if (currentSceneIndex < sceneNames.Length)
        {
            LoadScene(sceneNames[currentSceneIndex]);
        }
        else
        {
            Debug.LogWarning("No more levels to load.");
            // 可选：重置游戏或返回主菜单
            currentSceneIndex = 0;
            LoadScene(sceneNames[currentSceneIndex]);
        }
    }

    private void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name is null or empty!");
            return;
        }

        Debug.Log($"Loading Scene: {sceneName} (Index: {currentSceneIndex})");
        
        // 在加载新场景前进行资源清理
        Resources.UnloadUnusedAssets();
        System.GC.Collect();

        // 创建一个临时相机来防止场景切换时的黑屏
        GameObject tempCam = new GameObject("TempCamera");
        Camera cam = tempCam.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;
        DontDestroyOnLoad(tempCam);
        
        // 使用异步加载场景
        StartCoroutine(LoadSceneAsync(sceneName, tempCam));
    }

    private IEnumerator LoadSceneAsync(string sceneName, GameObject tempCam)
    {
        // 检查场景是否存在于构建设置中
        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError($"Scene '{sceneName}' does not exist in build settings!");
            Destroy(tempCam);
            yield break;
        }

        // 开始异步加载场景
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = true;

        // 等待场景加载完成
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // 等待一帧确保新场景的相机已经初始化
        yield return new WaitForEndOfFrame();

        // 销毁临时相机
        Destroy(tempCam);
    }

    private void Update()
    {
        // 检测按下 Esc 键
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitGame();
        }
    }

    public void ExitGame()
    {
        // 确保在退出前清理所有动画和资源
        DOTween.KillAll();
        DOTween.Clear();
        Resources.UnloadUnusedAssets();
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    // 添加一个方法来重置游戏状态
    private void ResetGameState()
    {
        currentScore = 0;
        currentSceneIndex = 0;
        // 重置其他需要重置的游戏状态...
    }

    private void OnEnable()
    {
        // 订阅场景加载事件
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // 取消订阅场景加载事件
        SceneManager.sceneLoaded -= OnSceneLoaded;
        
        // 清理动画
        DOTween.KillAll();
        DOTween.Clear();
    }

    private void OnDestroy()
    {
        // 确保所有动画都被清理
        DOTween.KillAll(true);
        
        // 清理持久的音频监听器
        if (persistentAudioListener != null)
        {
            Destroy(persistentAudioListener.gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 场景加载完成后的处理
        Debug.Log($"Scene loaded: {scene.name}");
        
        // 确保场景中有相机，但不添加新的 AudioListener
        if (Camera.main == null)
        {
            Debug.LogWarning("No main camera found in scene, creating one...");
            GameObject camObj = new GameObject("Main Camera");
            Camera cam = camObj.AddComponent<Camera>();
            cam.tag = "MainCamera";
            // 不再添加 AudioListener，因为我们使用持久的那个
        }
        
        // 禁用场景中的其他 AudioListener
        AudioListener[] listeners = FindObjectsOfType<AudioListener>();
        foreach (AudioListener listener in listeners)
        {
            if (listener != persistentAudioListener)
            {
                listener.enabled = false;
            }
        }
        
        // 如果是重新开始游戏，确保所有状态都被正确重置
        if (scene.name == "Level1")
        {
            ResetGameState();
            currentSceneIndex = 1;
        }
    }
}