using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScorePanel : MonoBehaviour
{
    public TextMeshProUGUI tmp;

    private void Awake()
    {
        if (tmp == null)
        {
            tmp = this.GetComponent<TextMeshProUGUI>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Register();
    }

    private void Register()
    {
        EventManager.GetInstance().AddEventListener<int>("ScoreChange", RefreshScore);
    }

    private void RefreshScore(int score)
    {
        tmp.text = "Score:" + score;
    }

    private void UnRegister()
    {
        EventManager.GetInstance().RemoveEventListener<int>("ScoreChange", RefreshScore);
    }

    private void OnDestroy()
    {
        UnRegister();
    }
}
