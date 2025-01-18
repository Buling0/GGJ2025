using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LoginPanel : BasePanel
{
    protected override void Awake()
    {
        //一定不能少base.Awake
        //因为需要执行父类里面的操作之后 额外添加事件 比如：找控件 添加事件监听
        base.Awake();
    }

    void Start()
    {
        Debug.Log("Login");
        //GetUIControl<Button>("btnStart").onClick.AddListener(ClickStart);
        //GetUIControl<Button>("btnQuit").onClick.AddListener(ClickQuit);

        /*
        EventTrigger trigger = GetUIControl<Button>("btnStart").gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        
        entry.eventID = EventTriggerType.Drag;
        entry.callback.AddListener(Drag);
        
        trigger.triggers.Add(entry);
        */
        
        UIManager.AddCustomEventListener(GetUIControl<Button>("btnStart"), EventTriggerType.PointerEnter, (data) =>
        {
            Debug.Log("enter");
        });
        
        UIManager.AddCustomEventListener(GetUIControl<Button>("btnStart"), EventTriggerType.PointerExit, (data) =>
        {
            Debug.Log("exit");
        });
    }

    private void Drag(BaseEventData data)
    {
        
    }
    
    public void InitInfo()
    {
        Debug.Log("chushihua");
    }
    
    public void ClickStart()
    {
        //UIManager.GetInstance().ShowPanel<Loading>("Loading");
    }

    public void ClickQuit()
    {
        //Debug.Log("q");
    }

    //重写basePanel的ShowUI方法
    public override void ShowUI()
    {
        base.ShowUI();
        //显示面板时 想要执行的逻辑 在UIManager里面回自动调用
    }
    
    public override void HideUI()
    {
        
    }

    protected override void OnClick(string btnName)
    {
        //base.OnClick(btnName);
        switch (btnName)
        {
            case "btnStart":
                Debug.Log("Start");
                break;
            case "btnQuit":
                Debug.Log("Quit");
                break;
        }
    }

    protected override void OnValueChanged(string toggleName, bool value)
    {
        //根据名字判断是哪一个单/多选框状态变化了
    }
}
