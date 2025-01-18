using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


/// <summary>
/// UI层级 可自定义调节
/// </summary>
public enum E_UI_Layer
{
    Bot,
    Mid,
    Top,
    System,
}

public class UIManager : BaseManager<UIManager>
{
    public Dictionary<string, BasePanel> panleDic = new Dictionary<string, BasePanel>();

    //记录UI的Canvas父对象 以便外部使用
    public RectTransform canvas;

    private Transform bot;
    private Transform mid;
    private Transform top;
    private Transform system;

    public UIManager()
    {
        //创建Canvas 并在过场时不会被销毁
        GameObject obj = ResManager.GetInstance().Laod<GameObject>("UIPrefabs/Canvas");
        //里氏转换原则
        canvas = obj.transform as RectTransform;
        GameObject.DontDestroyOnLoad(obj);

        bot = canvas.Find("Bot");
        mid = canvas.Find("Mid");
        top = canvas.Find("Top");
        system = canvas.Find("System");
        
        //创建EventSystem 并在过场时不会被销毁
        obj = ResManager.GetInstance().Laod<GameObject>("UIPrefabs/EventSystem");
        GameObject.DontDestroyOnLoad(obj);
    }
    
    public void ShowPanel<T>(string panelName, E_UI_Layer layer, UnityAction<T> callBack = null)where T : BasePanel
    {
        //如果面板已经存在
        if (panleDic.ContainsKey(panelName))
        {
            panleDic[panelName].ShowUI();
            if (callBack != null)
            {
                callBack(panleDic[panelName] as T);
            }
            return;
        }
        
        ResManager.GetInstance().LoadAsync<GameObject>("UIPrefabs/" + panelName, (obj) =>
        {
            //作为canvas的子对象 并设置位置
            Transform father = bot;
            switch (layer)
            {
                case E_UI_Layer.Mid:
                    father = mid;
                    break;
                case E_UI_Layer.Top:
                    father = top;
                    break;
                case E_UI_Layer.System:
                    father = system;
                    break;
            }
            
            //设置父对象 相对位置和大小
            obj.transform.SetParent(father);

            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            
            (obj.transform as RectTransform).offsetMax = Vector2.zero;
            (obj.transform as RectTransform).offsetMin = Vector2.zero;
            
            //得到预设体面板的脚本
            T panel = obj.GetComponent<T>();

            //处理面板创建完成后的逻辑
            if (callBack != null)
            {
                callBack(panel);
            }

            panel.ShowUI();
            
            //把面板存起来
            panleDic.Add(panelName, panel);
        });
    }
    
    public void HiedPanel(string panelName)
    {
        if (panleDic.ContainsKey(panelName))
        {
            panleDic[panelName].HideUI();
            GameObject.Destroy(panleDic[panelName].gameObject);
            panleDic.Remove(panelName);
        }
    }

    /// <summary>
    /// 获得一个已经存在的面板 方便外部调用
    /// </summary>
    public T GetPanel<T>(string name)where T : BasePanel
    {
        if (panleDic.ContainsKey(name))
        {
            return panleDic[name] as T;
        }

        return null;
    }

    //获得对应层级的父对象
    public Transform GetLayerFather(E_UI_Layer layer)
    {
        switch (layer)
        {
            case E_UI_Layer.Bot:
                return bot;
            case E_UI_Layer.Mid:
                return mid;
            case E_UI_Layer.Top:
                return top;
            case E_UI_Layer.System:
                return system;
        }

        return null;
    }
    
    public static void AddCustomEventListener(UIBehaviour control, EventTriggerType type, UnityAction<BaseEventData> callback)
    {
        //
        EventTrigger trigger = control.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = control.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(callback);
        
        trigger.triggers.Add(entry);
    }
}
