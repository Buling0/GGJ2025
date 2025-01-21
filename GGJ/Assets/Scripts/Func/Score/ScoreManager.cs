using System.Collections;
using System.Collections.Generic;
using Bubble;
using UnityEngine;

public class ScoreManager : BaseManager<ScoreManager>
{
    private int _score = 0;
    private int _comboCount = 0;  // 连续消除次数
    private float _comboTimer = 0f;  // 连续消除计时器
    private const float COMBO_TIME_LIMIT = 2f;  // 连续消除的时间限制（2秒内算连续）
    private const int BASE_SCORE = 8;  // 基础分数

    public int getScore()
    {
        return _score;
    }

    public void PlusScore(BubbleType bubbleType, int num)
    {
        // 检查是否是连续消除
        if (Time.time - _comboTimer <= COMBO_TIME_LIMIT)
        {
            _comboCount++;
        }
        else
        {
            _comboCount = 0;
        }
        _comboTimer = Time.time;

        // 计算基础分数
        int baseScore = BASE_SCORE;
        
        // 根据泡泡类型计算分数倍数
        float typeMultiplier = 1f;
        switch (bubbleType)
        {
            case BubbleType.Blue:
            case BubbleType.Red:
            case BubbleType.Yellow:
            case BubbleType.White:
                typeMultiplier = 1f;
                break;
            case BubbleType.Green:
            case BubbleType.Orange:
            case BubbleType.Purple:
                typeMultiplier = 2f;
                break;
        }

        // 计算连击倍数
        float comboMultiplier = 1f;
        if (_comboCount == 1) // 第二次连续消除
        {
            comboMultiplier = 2f;
        }
        else if (_comboCount >= 2) // 第三次及以上连续消除
        {
            comboMultiplier = 4f;
        }

        // 计算最终得分
        int addScore = Mathf.RoundToInt(baseScore * typeMultiplier * comboMultiplier);
        _score += addScore;

        // 触发分数变化事件
        EventManager.GetInstance().EventTrigger<int>("ScoreChange", _score);
        
        // 输出调试信息
        Debug.Log($"消除类型: {bubbleType}, 连击次数: {_comboCount}, 得分: {addScore}");
    }

    // 重置连击计数（在游戏重启或关卡重置时调用）
    public void ResetCombo()
    {
        _comboCount = 0;
        _comboTimer = 0f;
    }
}
