using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class EndPanel : BasePanel
{
    // UI组件引用
    private Button startBtn;
    private Button exitBtn;
    private Button creditsBtn;
    private CanvasGroup creditsPanel;  // 制作人员界面

    // 按钮缩放参数
    private readonly float scaleTime = 0.3f;
    private readonly float scaleFactor = 1.2f;
    private Vector3 originalScale = Vector3.one;

    // 在类的成员变量中添加
    private Button closeBtn;

    // 添加新的成员变量
    private RectTransform creditsContent;  // 制作人员界面的内容区域
    private float contentMoveDistance = 1000f;  // 移动距离
    private float contentAnimTime = 0.5f;  // 动画时间

    protected override void Awake()
    {
        base.Awake();
        // 获取组件引用
        startBtn = transform.Find("StartBtn").GetComponent<Button>();
        exitBtn = transform.Find("ExitBtn").GetComponent<Button>();
        creditsBtn = transform.Find("CreditsBtn").GetComponent<Button>();
        creditsPanel = transform.Find("CreditsPanel").GetComponent<CanvasGroup>();

        // 初始化制作人员界面为隐藏状态
        creditsPanel.alpha = 0;
        creditsPanel.interactable = false;
        creditsPanel.blocksRaycasts = false;

        // 添加Content的引用
        creditsContent = creditsPanel.transform.Find("TeamList").GetComponent<RectTransform>();
        
        // 初始化Content位置（在屏幕下方）
        creditsContent.anchoredPosition = new Vector2(0, -contentMoveDistance);

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
        // TODO: 在这里添加加载存档和开始游戏的逻辑
        Debug.Log("开始游戏");
        // 示例：加载游戏场景
        // SceneManager.LoadScene("GameScene");
    }

    // 退出游戏
    private void OnExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    // 显示制作人员界面
    private void OnShowCredits()
    {
        // 面板淡入
        creditsPanel.DOFade(1, 0.3f);
        creditsPanel.interactable = true;
        creditsPanel.blocksRaycasts = true;

        // 内容从下方移入
        creditsContent.DOAnchorPosY(0, contentAnimTime).SetEase(Ease.OutQuad);

        // 获取关闭按钮
        if (closeBtn == null)
        {
            closeBtn = creditsPanel.transform.Find("CloseBtn").GetComponent<Button>();
            closeBtn.onClick.RemoveAllListeners();
            closeBtn.onClick.AddListener(OnCloseCredits);
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

    private void OnDestroy()
    {
        // 确保在场景关闭时终止所有的 DOTween 动画
        DOTween.KillAll();
    }
}