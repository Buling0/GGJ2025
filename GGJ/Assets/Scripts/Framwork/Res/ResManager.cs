using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 资源加载模块
/// </summary>
public class ResManager : BaseManager<ResManager>
{
    public T Load<T>(string name) where T : Object
    {
        T res = Resources.Load<T>(name);
        //如果对象是GameObject类型 实例化后 直接返回外部 可以直接使用
        if (res is GameObject)
        {
            return GameObject.Instantiate(res);
        }
        else
        {
            return res;
        }
    }

    public void LoadAsync<T>(string name, UnityAction<T> callback) where T : Object
    {
        //开启异步加载的协程
        MonoManager.GetInstance().StartCoroutine(ReallyLoadAsync(name, callback));
    }

    private IEnumerator ReallyLoadAsync<T>(string name, UnityAction<T> callback) where T : Object
    {
        ResourceRequest res = Resources.LoadAsync<T>(name);
        yield return res;
        
        if(res.asset is GameObject)
        {
            //加载资源并类型转换
            callback(GameObject.Instantiate(res.asset) as T);
        }
        else
        {
            callback(res.asset as T);
        }
    }
    
}
