using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        //CheckKeyCode(KeyCode.S);
        CheckKeyCode(KeyCode.D);
        //CheckKeyCode(KeyCode.W);
        CheckKeyCode(KeyCode.Space);
    }
    
    private void CheckKeyCode(KeyCode key)
    {
        if (Input.GetKeyDown(key))
        {
            //事件中心模块，分发按下事件
            EventManager.GetInstance().EventTrigger("Key is Down", key);
            StartOrStopCheck(false);
        }

        if (Input.GetKeyUp(key))
        {
            //事件中心模块，分发抬起事件
            EventManager.GetInstance().EventTrigger("Key is Up", key);
            StartOrStopCheck(true);
        }
    }
}
