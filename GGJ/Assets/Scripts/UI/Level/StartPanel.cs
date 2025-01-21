using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections;

public class StartPanel : BasePanel
{
    // UI组件引用
    private Button startBtn;
    private Button exitBtn;
    private Button creditsBtn;
    private CanvasGroup creditsPanel;  // 制作人员界面

    private Button loadBtn;  // 操作说明按钮
    private CanvasGroup loadPanel;  // 操作说明界面

    private Button settingBtn;  // 游戏设置按钮
    private CanvasGroup settingPanel;  // 游戏设置界面

    // 按钮缩放参数
    private readonly float scaleTime = 0.3f;
    private readonly float scaleFactor = 1.2f;
    private Vector3 originalScale = Vector3.one;

    // 在类的成员变量中添加
    private Button closeBtn1;
    private Button closeBtn2;
    private Button closeBtn3;

    // 添加新的成员变量
    private RectTransform creditsContent;  // 制作人员界面的内容区域
    private RectTransform loadContent;  // 制作人员界面的内容区域
    private RectTransform settingContent;  // 制作人员界面的内容区域

    private float contentMoveDistance = 1000f;  // 移动距离
    private float contentAnimTime = 0.5f;  // 动画时间

    private Slider volumeSlider; // 添加音量滑动条的引用

    protected override void Awake()
    {
        base.Awake();
        // 获取组件引用
        startBtn = transform.Find("StartBtn").GetComponent<Button>();
        exitBtn = transform.Find("ExitBtn").GetComponent<Button>();
        creditsBtn = transform.Find("CreditsBtn").GetComponent<Button>();
        creditsPanel = transform.Find("CreditsPanel").GetComponent<CanvasGroup>();
        loadBtn = transform.Find("LoadBtn").GetComponent<Button>();
        loadPanel = transform.Find("LoadPanel").GetComponent<CanvasGroup>();
        if (loadPanel == null)
        {
            Debug.LogError("LoadPanel not found!");
        }
        else
        {
            loadContent = loadPanel.transform.Find("Instruction").GetComponent<RectTransform>();
            if (loadContent == null)
            {
                Debug.LogError("Instruction not found under LoadPanel!");
            }
        }
        settingBtn = transform.Find("SettingBtn").GetComponent<Button>();
        settingPanel = transform.Find("SettingPanel").GetComponent<CanvasGroup>();
        settingContent = settingPanel.transform.Find("Setting").GetComponent<RectTransform>();
        volumeSlider = settingContent.transform.Find("VolumeSlider").GetComponent<Slider>();
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
        else
        {
            Debug.LogError("VolumeSlider not found!");
        }

        // 初始化制作人员界面为隐藏状态
        creditsPanel.alpha = 0;
        creditsPanel.interactable = false;
        creditsPanel.blocksRaycasts = false;

        loadPanel.alpha = 0;
        loadPanel.interactable = false;
        loadPanel.blocksRaycasts = false;

        settingPanel.alpha = 0;
        settingPanel.interactable = false;
        settingPanel.blocksRaycasts = false;

        // 添加Content的引用
        creditsContent = creditsPanel.transform.Find("TeamList").GetComponent<RectTransform>();

        // 初始化Content位置（在屏幕下方）
        creditsContent.anchoredPosition = new Vector2(0, -contentMoveDistance);
        loadContent.anchoredPosition = new Vector2(0, -contentMoveDistance);
        settingContent.anchoredPosition = new Vector2(0, -contentMoveDistance);

        // 添加按钮事件监听
        RegisterButtonEvents();
    }

    private void RegisterButtonEvents()
    {
        // 开始按钮
        UIManager.AddCustomEventListener(startBtn, EventTriggerType.PointerEnter, (data) => OnPointerEnter(startBtn));
        UIManager.AddCustomEventListener(startBtn, EventTriggerType.PointerExit, (data) => OnPointerExit(startBtn));
        startBtn.onClick.AddListener(OnStartGame);

        // 退出按钮
        UIManager.AddCustomEventListener(exitBtn, EventTriggerType.PointerEnter, (data) => OnPointerEnter(exitBtn));
        UIManager.AddCustomEventListener(exitBtn, EventTriggerType.PointerExit, (data) => OnPointerExit(exitBtn));
        exitBtn.onClick.AddListener(OnExitGame);

        // 制作人员按钮
        UIManager.AddCustomEventListener(creditsBtn, EventTriggerType.PointerEnter, (data) => OnPointerEnter(creditsBtn));
        UIManager.AddCustomEventListener(creditsBtn, EventTriggerType.PointerExit, (data) => OnPointerExit(creditsBtn));
        creditsBtn.onClick.AddListener(OnShowCredits);

        // 继续游戏按钮
        UIManager.AddCustomEventListener(loadBtn, EventTriggerType.PointerEnter, (data) => OnPointerEnter(loadBtn));
        UIManager.AddCustomEventListener(loadBtn, EventTriggerType.PointerExit, (data) => OnPointerExit(loadBtn));
        loadBtn.onClick.AddListener(OnShowLoad);

        // 游戏设置按钮
        UIManager.AddCustomEventListener(settingBtn, EventTriggerType.PointerEnter, (data) => OnPointerEnter(settingBtn));
        UIManager.AddCustomEventListener(settingBtn, EventTriggerType.PointerExit, (data) => OnPointerExit(settingBtn));
        settingBtn.onClick.AddListener(OnShowSetting);
    }

    // 鼠标悬停效果
    private void OnPointerEnter(Button btn)
    {
        btn.transform.DOScale(originalScale * scaleFactor, scaleTime);
    }

    // 鼠标离开效果
    private void OnPointerExit(Button btn)
    {
        btn.transform.DOScale(originalScale, scaleTime);
    }

    // 开始游戏
    private void OnStartGame()
    {
        Debug.Log("Start button clicked");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }
        else
        {
            Debug.LogError("GameManager instance not found!");
        }
    }

    // 退出游戏
    public void OnExitGame()
    {
        Debug.Log("Exit button clicked");
        SafeExit();
    }

    // 显示制作人员界面
    private void OnShowCredits()
    {
        // 添加空引用检查
        if (creditsPanel == null)
        {
            Debug.LogError("Credits Panel is not assigned!");
            return;
        }
        if (creditsContent == null)
        {
            Debug.LogError("Credits Content is not assigned!");
            return;
        }

        // 面板淡入
        creditsPanel.DOFade(1, 0.3f);
        creditsPanel.interactable = true;
        creditsPanel.blocksRaycasts = true;

        // 内容从下方移入
        creditsContent.DOAnchorPosY(0, contentAnimTime).SetEase(Ease.OutQuad);

        // 获取关闭按钮
        if (closeBtn1 == null)
        {
            closeBtn1 = creditsPanel.transform.Find("CloseBtn1").GetComponent<Button>();
            if (closeBtn1 == null)
            {
                Debug.LogError("Could not find CloseBtn1!");
                return;
            }
            closeBtn1.onClick.RemoveAllListeners();
            closeBtn1.onClick.AddListener(OnCloseCredits);
        }
    }

    // 关闭制作人员界面
    private void OnCloseCredits()
    {
        // 内容向下移出
        creditsContent.DOAnchorPosY(-contentMoveDistance, contentAnimTime).SetEase(Ease.InQuad);

        // 面板淡出（延迟一点以配合移动动画）
        DOVirtual.DelayedCall(contentAnimTime * 0.5f, () => {
            creditsPanel.DOFade(0, 0.3f);
            creditsPanel.interactable = false;
            creditsPanel.blocksRaycasts = false;
        });
    }

    // 操作说明

    // 显示操作说明界面
    private void OnShowLoad()
    {
        Debug.Log("操作说明");
        // 面板淡入
        loadPanel.DOFade(1, 0.3f).SetEase(Ease.InOutQuad);
        loadPanel.interactable = true;
        loadPanel.blocksRaycasts = true;

        // 内容从下方移入
        loadContent.DOAnchorPosY(0, contentAnimTime).SetEase(Ease.OutQuad);

        // 获取关闭按钮
        if (closeBtn2 == null)
        {
            closeBtn2 = loadPanel.transform.Find("CloseBtn2").GetComponent<Button>();
            closeBtn2.onClick.RemoveAllListeners();
            closeBtn2.onClick.AddListener(OnCloseLoad);
        }
    }
    // 关闭操作说明界面
    private void OnCloseLoad()
    {
        // 内容向下移出
        loadContent.DOAnchorPosY(-contentMoveDistance, contentAnimTime).SetEase(Ease.InQuad);

        // 面板淡出（延迟一点以配合移动动画）
        DOVirtual.DelayedCall(contentAnimTime * 0.5f, () => {
            loadPanel.DOFade(0, 0.3f).SetEase(Ease.InOutQuad);
            loadPanel.interactable = false;
            loadPanel.blocksRaycasts = false;
        });
    }

    // 游戏设置
    // 显示游戏设置界面
    private void OnShowSetting()
    {
        Debug.Log("游戏设置");
        // 面板淡入
        settingPanel.DOFade(1, 0.3f);
        settingPanel.interactable = true;
        settingPanel.blocksRaycasts = true;

        // 内容从下方移入
        settingContent.DOAnchorPosY(0, contentAnimTime).SetEase(Ease.OutQuad);

        // 获取关闭按钮
        if (closeBtn3 == null)
        {
            closeBtn3 = settingPanel.transform.Find("CloseBtn3").GetComponent<Button>();
            closeBtn3.onClick.RemoveAllListeners();
            closeBtn3.onClick.AddListener(OnCloseSetting);
        }
    }
    // 关闭游戏设置界面
    private void OnCloseSetting()
    {
        // 内容向下移出
        settingContent.DOAnchorPosY(-contentMoveDistance, contentAnimTime).SetEase(Ease.InQuad);

        // 面板淡出（延迟一点以配合移动动画）
        DOVirtual.DelayedCall(contentAnimTime * 0.5f, () => {
            settingPanel.DOFade(0, 0.3f);
            settingPanel.interactable = false;
            settingPanel.blocksRaycasts = false;
        });
    }

    // 音量变化处理
    private void OnVolumeChanged(float value)
    {
        // 假设使用 AudioListener 来控制全局音量
        AudioListener.volume = value;
        Debug.Log($"Volume changed to: {value}");
    }

    // 添加事件注册方法
    private void Register()
    {
        // 如果有需要注册的事件，在这里添加
        if (EventManager.GetInstance() != null)
        {
            // 例如：
            // EventManager.GetInstance().AddEventListener<int>("SomeEvent", OnSomeEvent);
        }
    }

    // 添加事件注销方法
    private void UnRegister()
    {
        // 如果有需要注销的事件，在这里添加
        if (EventManager.GetInstance() != null)
        {
            // 例如：
            // EventManager.GetInstance().RemoveEventListener<int>("SomeEvent", OnSomeEvent);
        }
    }

    private void OnEnable()
    {
        Register();
    }

    private void OnDisable()
    {
        // 清理所有动画
        DOTween.KillAll();
        DOTween.Clear();
    }

    private void OnDestroy()
    {
        UnRegister();
        
        // 移除所有按钮事件监听
        if (startBtn != null) startBtn.onClick.RemoveAllListeners();
        if (exitBtn != null) exitBtn.onClick.RemoveAllListeners();
        if (creditsBtn != null) creditsBtn.onClick.RemoveAllListeners();
        if (loadBtn != null) loadBtn.onClick.RemoveAllListeners();
        if (settingBtn != null) settingBtn.onClick.RemoveAllListeners();
    }

    // 修改安全退出方法
    private void SafeExit()
    {
        // 先停止所有动画并等待完成
        DOTween.KillAll();
        DOTween.Clear();
        Resources.UnloadUnusedAssets();
        
        // 使用延迟调用来确保清理完成后再退出
        StartCoroutine(DelayedExit());
    }

    private IEnumerator DelayedExit()
    {
        // 等待一帧确保所有清理完成
        yield return null;
        
        // 然后调用游戏管理器的退出方法
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ExitGame();
        }
    }

    private void Update()
    {
        // 检测按下 Esc 键
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnExitGame();
        }
    }
}