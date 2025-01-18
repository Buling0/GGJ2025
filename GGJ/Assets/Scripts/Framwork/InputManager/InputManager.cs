using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 1.Input类
/// 2.事件中心模块
/// 3.公共Mono模块
/// </summary>

//思考题：如何实现改键的功能
public class InputManager : BaseManager<InputManager>
{
    private bool isStart = false;
    
    public InputManager()
    {
        MonoManager.GetInstance().AddUpdateListener(InputUpdate);
    }
    
    public void StartOrStopCheck(bool isstart)
    {
        isStart = isstart;
    }
    
    private void InputUpdate()
    {
        //判断是否可以进行输入检测
        if (!isStart)
        {
            return;
        }
        
        CheckKeyCode(KeyCode.A);
        CheckKeyCode(KeyCode.S);
        CheckKeyCode(KeyCode.D);
        CheckKeyCode(KeyCode.W);
    }
    
    private void CheckKeyCode(KeyCode key)
    {
        if (Input.GetKeyDown(key))
        {
            //事件中心模块，分发按下事件
            EventManager.GetInstance().EventTrigger("key is down", key);
        }

        if (Input.GetKeyUp(key))
        {
            //事件中心模块，分发抬起事件
            EventManager.GetInstance().EventTrigger("key is up", key);
        }
    }
}
