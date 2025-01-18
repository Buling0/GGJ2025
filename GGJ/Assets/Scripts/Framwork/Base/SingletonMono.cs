using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//继承了MonoBehaviour的单例模式，再Awake里面进行实例化
//需要手动或者Api挂载实例
//在切换场景时会删除对象
public class SingletonMono<T> : MonoBehaviour where T: MonoBehaviour
{
    private static T instance;

    public static T GetInstance()
    {
        return instance;
    }

    protected virtual void  Awake()
    {
        instance = this as T;
    }
}
