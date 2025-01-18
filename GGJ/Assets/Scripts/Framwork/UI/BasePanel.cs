using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 面板基类
/// 找到所有自己面板下的控件对象
/// 提供显示和隐藏的行为函数
/// 省去拖动赋值的行为 方便管理
/// </summary>
public class BasePanel : MonoBehaviour
{
    //通过里氏转换原则 存储所有的UI控件
    private Dictionary<string, List<UIBehaviour>> ctrUIDic = new Dictionary<string, List<UIBehaviour>>();
    
    protected virtual void Awake()
    {
        //UIBehaviour是所有UI控件的父类  运用里氏转换原则
        //this.GetComponentsInChildren<UIBehaviour>();
        FindChildernControl<Button>();
        FindChildernControl<Image>();
        FindChildernControl<Slider>();
        FindChildernControl<Text>();
        FindChildernControl<Toggle>();
        FindChildernControl<ScrollRect>();
        FindChildernControl<InputField>();
    }

    public virtual void ShowUI()
    {
        
    }

    public virtual void HideUI()
    {
        
    }

    /// <summary>
    /// 获取UI控件
    /// </summary>
    /// <param name="controlName"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    protected T GetUIControl<T>(string controlName) where T : UIBehaviour
    {
        if (ctrUIDic.ContainsKey(controlName))
        {
            for (int i = 0; i < ctrUIDic[controlName].Count; i++)
            {
                if (ctrUIDic[controlName][i] is T)
                {
                    return ctrUIDic[controlName][i] as T;
                }
            }
        }
        
        return null;
    }

    /// <summary>
    /// 找到子对象的UI控件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private void FindChildernControl<T>() where T : UIBehaviour
    {
        T[] UIControls = this.GetComponentsInChildren<T>();

        for (int i = 0; i < UIControls.Length; ++i)
        {
            string objName = UIControls[i].gameObject.name;
            if (ctrUIDic.ContainsKey(objName))
            {
                ctrUIDic[objName].Add(UIControls[i]);
            }
            else
            {
                ctrUIDic.Add(objName, new List<UIBehaviour>() { UIControls[i] });
            }

            //如果是按钮控件 直接添加监听事件
            //有点逆天多看看 lambda表达式
            if (UIControls[i] is Button)
            {
                (UIControls[i] as Button).onClick.AddListener(() =>
                {
                    OnClick(objName);
                });
            }

            //如果是单选框或者多选框
            if (UIControls[i] is Toggle)
            {
                (UIControls[i] as Toggle).onValueChanged.AddListener((value) =>
                {
                    OnValueChanged(objName, value);
                });
            }
        }
    }

    protected virtual void OnClick(string btnName)
    {
        
    }

    protected virtual void OnValueChanged(string toggleName, bool value)
    {
        
    }
}
