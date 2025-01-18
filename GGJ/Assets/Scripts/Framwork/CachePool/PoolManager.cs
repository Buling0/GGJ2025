using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PoolData
{
    //挂载对象的父节点
    public GameObject fatherObj;
    //对象的容器
    public List<GameObject> poolList;

    /// <summary>
    /// 构造函数，创建“抽屉”
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="poolObj"></param>
    public PoolData(GameObject obj, GameObject poolObj)
    {
        //创建父物体，名称为物体的名称
        fatherObj = new GameObject(obj.name);
        //设置父物体的父物体为缓存池
        fatherObj.transform.parent = poolObj.transform;

        //创建对象，并设置对象的父物体为该类型的父物体节点
        poolList = new List<GameObject>() { };
        
        PushObj(obj);
    }

    /// <summary>
    /// 创建对象并放入“抽屉”
    /// </summary>
    /// <param name="obj"></param>
    public void PushObj(GameObject obj)
    {
        //失活，将对象压入缓存池
        obj.SetActive(false);
        poolList.Add(obj);
        obj.transform.parent = fatherObj.transform;
    }
    
    /// <summary>
    /// 从缓存池中拿出对象并激活
    /// </summary>
    /// <returns></returns>
    public GameObject GetObj()
    {
        GameObject obj = null;
        obj = poolList[0];
        poolList.RemoveAt(0);
        obj.SetActive(true);
        obj.transform.parent = null;

        return obj;
    }
}


/// <summary>
/// 缓存池模块
/// Dictionary List
/// GameObject 和 Resources 两个公共类的API
/// </summary>
public class PoolManager : BaseManager<PoolManager>
{
    //缓存池容器
    public Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();
    
    //作为缓存池的父节点
    private GameObject PoolRoot;

    public void GetObj(string name, UnityAction<GameObject> callback)
    {
        if (poolDic.ContainsKey(name) && poolDic[name].poolList.Count > 0)
        {
            callback(poolDic[name].GetObj());
        }
        else
        {
            //异步加载方式
            
            //创建对象给外部使用
            ResManager.GetInstance().LoadAsync<GameObject>(name, (obj) =>
            {
                obj.name = name;
                callback(obj);
            });
        }
    }

    public void PushObj(string name, GameObject obj)
    {
        if (PoolRoot == null)
        {
            PoolRoot = new GameObject("PoolRoot");
        }

        if (poolDic.ContainsKey(name))
        {
            poolDic[name].PushObj(obj);
        }
        //无该物体的缓存池，创建一个抽屉
        else
        {
            poolDic.Add(name, new PoolData(obj, PoolRoot));
        }
    }

    public void Clear()
    {
        poolDic.Clear();
        PoolRoot = null;
    }
}
