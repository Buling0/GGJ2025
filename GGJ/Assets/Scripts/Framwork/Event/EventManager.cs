using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 
/// </summary>
public interface IEventInfo
{
    
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public class EventInfo<T> : IEventInfo
{
    public UnityAction<T> actions;

    public EventInfo(UnityAction<T> action)
    {
        actions += action;
    }
}

/// <summary>
/// 
/// </summary>
public class EventInfo : IEventInfo
{
    public UnityAction actions;

    public EventInfo(UnityAction action)
    {
        actions += action;
    }
}


/// <summary>
/// 事件中心
/// 单例模式设计对象
/// Dictionary, 委托，观察者设计模式
/// 泛型
/// </summary>
public class EventManager : BaseManager<EventManager>
{
    //key —— 事件的名称
    //value —— 监听事件对应的函数
    //数组也可以用object表示，可以传入多个参数
    private Dictionary<string, IEventInfo> eventDic = new Dictionary<string, IEventInfo>();

    public void test()
    {
        
    }
    
    public void AddEventListener<T>(string name, UnityAction<T> action)
    {
        //判断字典中是否有该事件
        if (eventDic.ContainsKey(name))
        {
            //添加委托
            (eventDic[name] as EventInfo<T>).actions += action;
        }
        else
        {
            eventDic.Add(name, new EventInfo<T>(action));
        }
    }
    
    public void AddEventListener(string name, UnityAction action)
    {
        //判断字典中是否有该事件
        if (eventDic.ContainsKey(name))
        {
            //添加委托
            (eventDic[name] as EventInfo).actions += action;
        }
        else
        {
            eventDic.Add(name, new EventInfo(action));
        }
    }

    public void EventTrigger<T>(string name, T info)
    {
        if (eventDic.ContainsKey(name))
        {
            if ((eventDic[name] as EventInfo<T>).actions != null)
            {
                (eventDic[name] as EventInfo<T>).actions.Invoke(info);
            }
        }
    }
    
    public void EventTrigger(string name)
    {
        if (eventDic.ContainsKey(name))
        {
            if ((eventDic[name] as EventInfo).actions != null)
            {
                (eventDic[name] as EventInfo).actions.Invoke();
            }
        }
    }
    
    public void RemoveEventListener<T>(string name, UnityAction<T> action)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T>).actions -= action;
        }
    }
    
    public void RemoveEventListener(string name, UnityAction action)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo).actions -= action;
        }
    }

    public void EventClear()
    {
        eventDic.Clear();
    }
    
}
