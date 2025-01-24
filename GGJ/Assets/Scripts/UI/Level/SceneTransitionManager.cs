using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneTransitionManager : MonoBehaviour
{
    private static SceneTransitionManager instance;
    public static SceneTransitionManager Instance
    {
        get
        {
            if (instance == null)
            {
                // 创建一个新的GameObject并添加SceneTransitionManager组件
                GameObject go = new GameObject("SceneTransitionManager");
                instance = go.AddComponent<SceneTransitionManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    private CanvasGroup fadeCanvasGroup;
    private Canvas fadeCanvas;
    public float fadeDuration = 0.5f;
    private bool isTransitioning = false;
    private bool isInitialized = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeTransitionCanvas();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitializeTransitionCanvas()
    {
        if (isInitialized) return;

        try
        {
            // 创建和设置Canvas
            fadeCanvas = gameObject.AddComponent<Canvas>();
            fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            fadeCanvas.sortingOrder = 999; // 确保在最上层

            // 添加GraphicRaycaster组件
            gameObject.AddComponent<GraphicRaycaster>();

            // 添加CanvasScaler组件
            CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            // 创建黑色背景面板
            GameObject panelObj = new GameObject("FadePanel");
            panelObj.transform.SetParent(transform, false);
            Image fadeImage = panelObj.AddComponent<Image>();
            fadeImage.color = Color.black;
            fadeImage.raycastTarget = false;

            // 设置面板大小为全屏
            RectTransform panelRect = panelObj.GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.sizeDelta = Vector2.zero;
            panelRect.localScale = Vector3.one;

            // 添加CanvasGroup组件
            fadeCanvasGroup = panelObj.AddComponent<CanvasGroup>();
            fadeCanvasGroup.alpha = 1f;

            isInitialized = true;
            StartFadeIn();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"初始化转场画布时出错: {e.Message}");
        }
    }

    private void StartFadeIn()
    {
        if (fadeCanvasGroup == null) return;

        isTransitioning = true;
        fadeCanvasGroup.alpha = 1f;
        fadeCanvasGroup.DOFade(0f, fadeDuration)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true)
            .OnComplete(() => {
                fadeCanvasGroup.alpha = 0f;
                isTransitioning = false;
            });
    }

    public void LoadScene(string sceneName)
    {
        if (!isTransitioning && fadeCanvasGroup != null)
        {
            StartCoroutine(HandleTransitionEffect());
        }
    }

    private System.Collections.IEnumerator HandleTransitionEffect()
    {
        if (fadeCanvasGroup == null) yield break;

        isTransitioning = true;

        // 淡出效果
        Tween fadeTween = fadeCanvasGroup.DOFade(1f, fadeDuration)
            .SetEase(Ease.InQuad)
            .SetUpdate(true);

        yield return fadeTween.WaitForCompletion();

        // 等待一小段时间确保淡出完成
        yield return new WaitForSeconds(0.1f);

        // 淡入效果
        fadeCanvasGroup.DOFade(0f, fadeDuration)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true)
            .OnComplete(() => {
                fadeCanvasGroup.alpha = 0f;
                isTransitioning = false;
            });
    }

    private void OnDestroy()
    {
        if (fadeCanvasGroup != null)
        {
            DOTween.Kill(fadeCanvasGroup);
        }
    }
} 