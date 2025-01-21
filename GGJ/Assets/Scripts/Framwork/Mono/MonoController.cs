using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Mono管理者
/// 1.声明周期函数
/// 2.事件
/// 3.协程
/// </summary>
public class MonoController : MonoBehaviour
{
    private event UnityAction updateEvent;
    
    private void Awake()
    {
        // 确保在场景切换时不被销毁
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        // 移除这行，因为在 Awake 中已经调用了
        // DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (updateEvent != null)
        {
            updateEvent();
        }
    }

    /// <summary>
    /// 给外部用于添加帧更新的函数
    /// </summary>
    /// <param name="fun"></param>
    public void AddUpdateListener(UnityAction fun)
    {
        updateEvent += fun;
    }
    
    /// <summary>
    /// 给外部用于移除帧更新的函数
    /// </summary>
    /// <param name="fun"></param>
    public void RemoveUpdateListener(UnityAction fun)
    {
        updateEvent -= fun;
    }

    // 添加清理所有更新事件的方法
    public void ClearAllUpdateListeners()
    {
        updateEvent = null;
    }
}
