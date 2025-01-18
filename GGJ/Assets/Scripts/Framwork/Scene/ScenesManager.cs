using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景切换模块
/// 1.场景异步加载
/// 2.协程
/// 3.委托
/// </summary>
public class ScenesManager : BaseManager<SceneManager>
{
    /// <summary>
    /// 切换场景  同步加载
    /// </summary>
    /// <param name="name"></param>
    /// <param name="func"></param>
    public void LoadScene(string name, UnityAction fun)
    {
        //场景同步加载
        SceneManager.LoadScene(name);
        //加载完成后才会执行fun
        fun();
    }

    /// <summary>
    /// 异步加载
    /// 提供给外部的 异步加载的接口方法
    /// </summary>
    /// <param name="name"></param>
    /// <param name="fun"></param>
    public void LoadSceneAsyn(string name, UnityAction fun)
    {
        //通过MonoManager开启协程
        //即使SceneManager没有继承MonoBehaviour也能使用协程的函数
        MonoManager.GetInstance().StartCoroutine(ReallyLoadSceneAsyn(name, fun));
    }

    /// <summary>
    /// 协程异步加载
    /// </summary>
    /// <param name="name"></param>
    /// <param name="fun"></param>
    /// <returns></returns>
    private IEnumerator ReallyLoadSceneAsyn(string name, UnityAction fun)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(name);
        //获得加载场景的进度
        while (!ao.isDone)
        {
            //事件中心向外分发进度情况
            EventManager.GetInstance().EventTrigger("进度条更新", ao.progress);
            //更新进度条
            yield return ao.progress;
        }
        //ao.progress;
        //yield return ao;

        fun();
    }
}
