using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//继承了MonoBehaviour，直接创建新的对象
//不需要手动挂载实例
//在切换场景时会删除对象
public class SingletonAutoMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T GetInstance()
    {
        if (instance == null)
        {
            GameObject obj = new GameObject();
            //设置对象名字为脚本名
            obj.name = typeof(T).ToString();
            //让单例对象在过场景时不会删除
            DontDestroyOnLoad(obj);
            
            instance = obj.AddComponent<T>();
        }

        return instance;
    }
}
