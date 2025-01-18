using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAudio : MonoBehaviour
{
    public float v = 1;

    private AudioSource source;
    
    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 100, 100), "PlayAudio"))
        {
            AudioManager.GetInstance().PalyBGM("bgm");
        }
        
        if (GUI.Button(new Rect(0, 100, 100, 100), "PauseAudio"))
        {
            AudioManager.GetInstance().PauseBGM();
        }
        
        if (GUI.Button(new Rect(0, 200, 100, 100), "StopAudio"))
        {
            AudioManager.GetInstance().StopBGM();
        }
        
        //AudioManager.GetInstance().ChangerbgmValue(v);
        
        
        if (GUI.Button(new Rect(0, 300, 100, 100), "PlayAudio"))
        {
            AudioManager.GetInstance().PlayAudio("bgm", false, (s) =>
            {
                source = s;
            });
        }
        
        if (GUI.Button(new Rect(0, 400, 100, 100), "StopAudio"))
        {
            AudioManager.GetInstance().StopAudio(source);
        }
        
    }
}
