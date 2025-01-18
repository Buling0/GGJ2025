using System.Collections;
using System.Collections.Generic;
using Bubble;
using UnityEngine;

public class ScoreManager : BaseManager<ScoreManager>
{
    private int _score = 0;

    public int getScore()
    {
        int s = _score;
        return s;
    }

    public void PlusScore(BubbleType bubbleType, int num)
    {
        switch (bubbleType)
        {
            case BubbleType.Blue:
            case BubbleType.Red:
            case BubbleType.Yellow:
            case BubbleType.White:
                _score += num;
                break;
            case BubbleType.Green:
            case BubbleType.Orange:
            case BubbleType.Purple:
                _score += num;
                break;
        }
        
        //其他地方显示分数
        EventManager.GetInstance().EventTrigger<int>("ScoreChange", _score);
    }
}
