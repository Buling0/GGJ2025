using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections;
using Bubble;

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
    private int[] levelTargetScores = { 0, 32, 32, 64, 64, 128, 128, 256, 256, 512, 0 };
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
        Debug.Log("Starting scene cleanup...");
        
        // 1. 先清理UI和游戏对象
        CleanupGameObjects();
        
        // 2. 清理特定事件（保留系统事件）
        CleanupEvents();
        
        // 3. 清理动画
        CleanupAnimations();
    }

    private void CleanupGameObjects()
    {
        // 获取所有游戏对象
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        
        foreach (GameObject obj in allObjects)
        {
            // 跳过需要保留的对象
            if (ShouldPreserveObject(obj))
                continue;

            // 销毁其他对象
            Destroy(obj);
        }
    }

    private bool ShouldPreserveObject(GameObject obj)
    {
        // 保留的对象类型
        if (obj == gameObject || // GameManager自身
            obj.CompareTag("DontDestroy") || // 标记为不销毁的对象
            obj.GetComponent<AudioListener>() == persistentAudioListener || // 持久的音频监听器
            obj.name.Contains("Manager") || // 所有管理器
            obj.GetComponent<MonoController>() != null || // MonoController
            obj.GetComponent<SoundEffectsManager>() != null) // 音效管理器
        {
            return true;
        }

        // 检查是否是泡泡相关的重要对象
        if (obj.GetComponent<BubbleEntity>() != null || // 泡泡实体
            obj.GetComponent<BubbleShooter>() != null) // 泡泡发射器
        {
            return true;
        }

        return false;
    }

    private void CleanupEvents()
    {
        if (EventManager.GetInstance() != null)
        {
            // 只清理游戏相关的事件，保留系统事件
            EventManager.GetInstance().RemoveEventListener<int>("ScoreChange", null);
            // 可以添加其他需要清理的特定事件
        }
    }

    private void CleanupAnimations()
    {
        // 停止当前场景的动画，但不清理整个DOTween系统
        DOTween.KillAll(false); // false表示不完成动画直接停止
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
        Debug.Log($"Loading next level. Current scene index: {currentSceneIndex}");
        currentSceneIndex++;
        
        if (currentSceneIndex < sceneNames.Length)
        {
            // 温和的清理
            CleanupCurrentScene();
            
            // 使用协程加载下一个场景
            StartCoroutine(LoadNextLevelAsync(sceneNames[currentSceneIndex]));
        }
        else
        {
            Debug.Log("Game completed, returning to start");
            currentSceneIndex = 0;
            LoadScene(sceneNames[currentSceneIndex]);
        }
    }

    private IEnumerator LoadNextLevelAsync(string sceneName)
    {
        // 等待一帧确保清理完成
        yield return null;

        // 创建临时相机
        GameObject tempCam = CreateTemporaryCamera();

        // 加载新场景
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // 等待场景初始化
        yield return new WaitForEndOfFrame();

        // 清理临时相机
        if (tempCam != null)
            Destroy(tempCam);

        // 初始化新场景
        InitializeNewScene();
    }

    private GameObject CreateTemporaryCamera()
    {
        GameObject tempCam = new GameObject("TempCamera");
        Camera cam = tempCam.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;
        DontDestroyOnLoad(tempCam);
        return tempCam;
    }

    private void InitializeNewScene()
    {
        // 确保场景有主相机
        if (Camera.main == null)
        {
            CreateMainCamera();
        }

        // 禁用多余的AudioListener
        DisableExtraAudioListeners();

        // 重新初始化必要的管理器
        InitializeManagers();
    }

    private void CreateMainCamera()
    {
        GameObject camObj = new GameObject("Main Camera");
        Camera cam = camObj.AddComponent<Camera>();
        cam.tag = "MainCamera";
    }

    private void DisableExtraAudioListeners()
    {
        AudioListener[] listeners = FindObjectsOfType<AudioListener>();
        foreach (AudioListener listener in listeners)
        {
            if (listener != persistentAudioListener)
            {
                listener.enabled = false;
            }
        }
    }

    private void InitializeManagers()
    {
        // 确保所有必要的管理器都被正确初始化
        if (BubbleManager.GetInstance() != null)
        {
            // 初始化泡泡管理器的状态
        }

        if (SoundEffectsManager.Instance != null)
        {
            // 重新设置音效管理器的状态
        }

        // 可以添加其他管理器的初始化
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
        // 重置连击计数
        ScoreManager.GetInstance().ResetCombo();
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