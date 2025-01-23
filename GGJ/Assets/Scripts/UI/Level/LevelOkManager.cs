using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelOkManager : MonoBehaviour
{
    public Button okButton;
    private EventTrigger eventTrigger;

    private void Start()
    {
        if (okButton != null)
        {
            okButton.onClick.AddListener(OnOkButtonClicked);
            SetupEventTrigger();
        }
        else
        {
            Debug.LogError("OK Button is not assigned in the inspector.");
        }
    }

    private void SetupEventTrigger()
    {
        eventTrigger = okButton.GetComponent<EventTrigger>();
        if (eventTrigger == null)
        {
            // 如果没有EventTrigger，添加一个
            eventTrigger = okButton.gameObject.AddComponent<EventTrigger>();
        }

        // 清除现有的triggers
        eventTrigger.triggers.Clear();

        // 添加鼠标悬停事件
        EventTrigger.Entry enterEntry = new EventTrigger.Entry();
        enterEntry.eventID = EventTriggerType.PointerEnter;
        enterEntry.callback.AddListener((data) => SoundEffectsManager.Instance.PlayHoverSound());
        eventTrigger.triggers.Add(enterEntry);

        // 添加点击事件
        EventTrigger.Entry clickEntry = new EventTrigger.Entry();
        clickEntry.eventID = EventTriggerType.PointerClick;
        clickEntry.callback.AddListener((data) => SoundEffectsManager.Instance.PlayClickSound());
        eventTrigger.triggers.Add(clickEntry);
    }

    private void OnEnable()
    {
        // 场景切换后重新设置事件触发器
        if (okButton != null)
        {
            SetupEventTrigger();
        }
    }

    public void OnOkButtonClicked()
    {
        Debug.Log("OK Button clicked.");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadNextLevel();
        }
        else
        {
            Debug.LogError("GameManager instance not found.");
        }
    }
} 