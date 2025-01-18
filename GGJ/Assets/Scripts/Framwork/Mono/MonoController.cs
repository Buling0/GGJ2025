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
    public event UnityAction updateEvent;
    
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
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
    public void RemoveUpdateLisener(UnityAction fun)
    {
        updateEvent -= fun;
    }
}
