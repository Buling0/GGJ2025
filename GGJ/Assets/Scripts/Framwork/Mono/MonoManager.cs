using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 可以提供给外部添加帧更新的方法
/// 可以提供给外部协程的方法
/// </summary>
public class MonoManager : BaseManager<MonoManager>
{
    private MonoController controller;

    public MonoManager()
    {
        //保证了MonoController的唯一性
        GameObject obj = new GameObject("MonoController");
        controller = obj.AddComponent<MonoController>();
        // 确保在场景切换时不被销毁
        GameObject.DontDestroyOnLoad(obj);
    }
    
    /// <summary>
    /// 给外部用于添加帧更新的函数
    /// </summary>
    /// <param name="fun"></param>
    public void AddUpdateListener(UnityAction fun)
    {
        if (controller != null)
            controller.AddUpdateListener(fun);
    }
    
    /// <summary>
    /// 给外部用于移除帧更新的函数
    /// </summary>
    /// <param name="fun"></param>
    public void RemoveUpdateListener(UnityAction fun)
    {
        if (controller != null)
            controller.RemoveUpdateListener(fun);
    }

    #region 协程相关函数封装
    
    /// <summary>
    /// 封装开启协程的方法，其他的方法同理，就不一一演示了
    /// </summary>
    /// <param name="routine"></param>
    /// <returns></returns>
    public Coroutine StartCoroutine(IEnumerator routine)
    {
        if (controller != null)
            return controller.StartCoroutine(routine);
        return null;
    }

    public void StopCoroutine(Coroutine routine)
    {
        if (controller != null && routine != null)
            controller.StopCoroutine(routine);
    }

    public void StopAllCoroutines()
    {
        if (controller != null)
            controller.StopAllCoroutines();
    }

    // 修改清理方法
    public void Clear()
    {
        if (controller != null)
        {
            // 停止所有协程
            controller.StopAllCoroutines();
            
            // 清理所有更新事件
            controller.ClearAllUpdateListeners();
        }
    }

    #endregion

    
    
}
