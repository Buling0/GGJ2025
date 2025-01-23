using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

namespace Bubble
{
    public enum BubbleType
    {
        White,
        Red,
        Yellow,
        Blue,
        Orange,
        Green,
        Purple
    }

    //只能用于泡泡实体类，泛用性低，可考虑泛型类
    public class BubbleDic
    {
        //private bool isBeLiminate = false;
        private Dictionary<BubbleType, List<BubbleEntity>> Dic = new Dictionary<BubbleType, List<BubbleEntity>>();

        public void Add(BubbleEntity bubbleEntity)
        {
            BubbleType bubbleType = bubbleEntity.GetBubbleType();
            if (Dic.ContainsKey(bubbleType))
            {
                Dic[bubbleType].Add(bubbleEntity);
            }
            else
            {
                Dic.Add(bubbleType, new List<BubbleEntity>() { bubbleEntity });
            }
        }

        public int CheckTypeNum(BubbleType bubbleType)
        {
            if (Dic.ContainsKey(bubbleType))
            {
                return Dic[bubbleType].Count;
            }

            return 0;
        }

        public void Refresh(BubbleType bubbleType)
        {
            if (Dic.ContainsKey(bubbleType))
            {
                List<BubbleEntity> list = Dic[bubbleType];
                Dic.Remove(bubbleType);
                foreach (BubbleEntity bubble in list)
                {
                    if (bubble != null)
                    {
                        this.Add(bubble);
                    }
                }
            }
        }

        public void EliminateByType(BubbleType bubbleType, out int num)
        {
            if (!Dic.ContainsKey(bubbleType))
            {
                num = 0;
                return;
            }

            List<BubbleEntity> list = Dic[bubbleType];
            
            // 如果数量不足3个，不进行消除
            if (list.Count < 2)  // 加上触发消除的泡泡共3个
            {
                num = 0;
                return;
            }

            // 获取所有相连的相同类型泡泡
            List<BubbleEntity> allConnectedBubbles = new List<BubbleEntity>(list);
            
            // 确保包含触发消除的泡泡
            foreach (var bubble in list)
            {
                if (bubble != null)
                {
                    var connectedBubbles = bubble.AdjoinBubbleDic.BeEliminate(bubbleType);
                    if (connectedBubbles != null)
                    {
                        foreach (var connectedBubble in connectedBubbles)
                        {
                            if (!allConnectedBubbles.Contains(connectedBubble))
                            {
                                allConnectedBubbles.Add(connectedBubble);
                            }
                        }
                    }
                }
            }

            num = allConnectedBubbles.Count;
            
            // 触发分数计算
            ScoreManager.GetInstance().PlusScore(bubbleType, num);
            
            // 销毁所有匹配的泡泡
            BubbleManager.GetInstance().DestoryBubbleList(allConnectedBubbles);
        }

        public List<BubbleEntity> BeEliminate(BubbleType bubbleType)
        {
            if (!Dic.ContainsKey(bubbleType))
            {
                return null;
            }
            
            List<BubbleEntity> list = Dic[bubbleType];
            
            // 确保所有相同类型的泡泡都被标记为待销毁
            foreach (var bubble in list)
            {
                if (bubble != null && !bubble.isDestorying)
                {
                    bubble.isDestorying = true;
                }
            }
            
            return list;
        }

        public void Remove(BubbleEntity bubbleEntity)
        {
            BubbleType bubbleType = bubbleEntity.GetBubbleType();
            if (!Dic.ContainsKey(bubbleType))
            {
                return;
            }

            Dic[bubbleType].Remove(bubbleEntity);
            if (!Dic.ContainsKey(bubbleType) || Dic[bubbleType].Count == 0)
            {
                Dic.Remove(bubbleType);
                GCManager.GetInstance().StartGC();
            }
        }

        public void ClearByType(BubbleType bubbleType)
        {
            Dic[bubbleType].Clear();
            Dic.Remove(bubbleType);
            GCManager.GetInstance().StartGC();
        }

        public void Clear()
        {
            Dic.Clear();
        }

        ~BubbleDic()
        {
            Dic.Clear();
        }
    }


    public class BubbleEntity : MonoBehaviour
    {
        [SerializeField] private BubbleType _bubbleType;
        private Rigidbody2D _rigidbody2D;
        private Image _image;
        public BubbleDic AdjoinBubbleDic = new BubbleDic();
        public bool isDestorying = false;
        public bool isBlending = false;
        private bool isReadyToShoot = true; // 添加标记来判断是否处于待发射状态

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            
            // 优化物理设置
            if (_rigidbody2D != null)
            {
                _rigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                _rigidbody2D.sleepMode = RigidbodySleepMode2D.NeverSleep;
                _rigidbody2D.interpolation = RigidbodyInterpolation2D.Interpolate;
                
                // 初始时禁用物理模拟
                _rigidbody2D.simulated = false;
                _rigidbody2D.gravityScale = 0;
            }
            
            Register();
        }

        private void Register()
        {
            EventManager.GetInstance().AddEventListener<BubbleEntity>("Blend", Blend);
            EventManager.GetInstance().AddEventListener<BubbleType>("BlendDone", BlendOrEliminateDone);
            EventManager.GetInstance().AddEventListener<BubbleEntity>("Eliminate", Eliminate);
            EventManager.GetInstance().AddEventListener<BubbleType>("EliminateDone", BlendOrEliminateDone);
        }

        public BubbleType GetBubbleType()
        {
            BubbleType bubbleType = _bubbleType;
            return bubbleType;
        }

        // 添加准备发射的方法
        public void PrepareForShoot()
        {
            if (_rigidbody2D != null)
            {
                _rigidbody2D.simulated = false;
                _rigidbody2D.gravityScale = 0;
                isReadyToShoot = true;
            }
        }

        //修改发射方法
        public void AddForce(Vector2 dir, float fv)
        {
            if (_rigidbody2D == null || !isReadyToShoot) return;
            
            // 发射时启用物理模拟和重力
            _rigidbody2D.simulated = true;
            _rigidbody2D.gravityScale = -1; // 设置为预制体中的值
            
            // 添加发射力
            _rigidbody2D.AddForce(dir * fv, ForceMode2D.Impulse);
            isReadyToShoot = false; // 标记已发射
        }

        //碰撞开始 把碰撞物体加入字典
        private void OnCollisionEnter2D(Collision2D bubble)
        {
            if (bubble == null) return;

            BubbleEntity bubbleEntity = bubble.gameObject.GetComponent<BubbleEntity>();
            if (bubbleEntity == null) return;

            // 添加到相邻泡泡字典
            AdjoinBubbleDic.Add(bubbleEntity);
            
            // 检查是否可以消除（三消）
            if (CheckCanEliminate())
            {
                // 立即触发消除
                EventManager.GetInstance().EventTrigger<BubbleEntity>("Eliminate", this);
                return;
            }
        }

        //碰撞结束 把物体移除字典
        private void OnCollisionExit2D(Collision2D bubble)
        {
            if (bubble == null)
            {
                return;
            }

            BubbleEntity bubbleEntity = bubble.gameObject.GetComponent<BubbleEntity>();
            if (bubbleEntity == null)
            {
                return;
            }

            AdjoinBubbleDic.Remove(bubbleEntity);
        }

        //颜色合成检测
        private void CheckCanBlend(BubbleEntity bubbleEntity, out bool canBlend)
        {
            canBlend = false;
            BubbleType bt = bubbleEntity.GetBubbleType();
            if (_bubbleType == BubbleType.White || bt == _bubbleType || bt == BubbleType.White)
            {
                return;
            }

            //混合的泡泡和发射器生成的泡泡不一样
            //应该是在原地合并，不需要发射
            switch (_bubbleType)
            {
                case BubbleType.Blue:
                    if (bt == BubbleType.Red)
                    {
                        canBlend = true;
                        _bubbleType = BubbleType.Purple;
                    }
                    else if (bt == BubbleType.Yellow)
                    {
                        canBlend = true;
                        _bubbleType = BubbleType.Green;
                    }

                    break;
                case BubbleType.Red:
                    if (bt == BubbleType.Blue)
                    {
                        canBlend = true;
                        _bubbleType = BubbleType.Purple;
                    }
                    else if (bt == BubbleType.Yellow)
                    {
                        canBlend = true;
                        _bubbleType = BubbleType.Orange;
                    }

                    break;
                case BubbleType.Yellow:
                    if (bt == BubbleType.Red)
                    {
                        canBlend = true;
                        _bubbleType = BubbleType.Orange;
                    }
                    else if (bt == BubbleType.Blue)
                    {
                        canBlend = true;
                        _bubbleType = BubbleType.Green;
                    }

                    break;
                default:
                    canBlend = false;
                    return;
            }
        }

        //修改消除检测逻辑
        private bool CheckCanEliminate()
        {
            // 检查相同颜色的泡泡数量是否达到3个
            if (AdjoinBubbleDic.CheckTypeNum(_bubbleType) >= 2)  // 已经有2个相邻，加上自己就是3个
            {
                return true;
            }
            return false;
        }

        //进行颜色混合
        private void Blend(BubbleEntity bubbleEntity)
        {
            if (bubbleEntity != this && isBlending)
            {
                if (_bubbleType == BubbleType.Green || _bubbleType == BubbleType.Purple ||
                    _bubbleType == BubbleType.Orange)
                {
                    Debug.Log("Blend");
                    Transform p1 = bubbleEntity.gameObject.transform;
                    Transform p2 = this.gameObject.transform;
                    Vector3 posi = (p1.position + p2.position) / 2;
                    BubbleManager.GetInstance().CreateBubbleByBlend(_bubbleType, p1.parent, posi);
                }
            }
        }

        //修改消除逻辑，加快处理速度
        public void Eliminate(BubbleEntity bubbleEntity)
        {
            if (bubbleEntity == this && !isDestorying)
            {
                isDestorying = true;
                int num;
                
                // 立即开始消除
                AdjoinBubbleDic.EliminateByType(bubbleEntity.GetBubbleType(), out num);
                Debug.Log($"消除了{num}个{bubbleEntity.GetBubbleType()}泡泡");
                
                // 只有在实际消除了泡泡时才销毁自己
                if (num > 0)
                {
                    DestorySelf();
                }
                else
                {
                    isDestorying = false;
                }
            }
        }

        private void BlendOrEliminateDone(BubbleType bubbleType)
        {
            AdjoinBubbleDic.Refresh(bubbleType);
            if (isBlending)
            {
                DestorySelf();
            }
        }

        private void UnRegister()
        {
            EventManager.GetInstance().RemoveEventListener<BubbleEntity>("Blend", Blend);
            EventManager.GetInstance().RemoveEventListener<BubbleType>("BlendDone", BlendOrEliminateDone);
            EventManager.GetInstance().RemoveEventListener<BubbleEntity>("Eliminate", Eliminate);
            EventManager.GetInstance().RemoveEventListener<BubbleType>("EliminateDone", BlendOrEliminateDone);
        }

        public void DestorySelf()
        {
            if (gameObject == null) return;
            
            // 立即禁用碰撞和物理
            if (_rigidbody2D != null)
            {
                _rigidbody2D.simulated = false;
            }
            
            // 立即处理销毁
            gameObject.SetActive(false);
            AdjoinBubbleDic.Clear();
            UnRegister();
            
            _rigidbody2D = null;
            Destroy(gameObject, 0.005f);  // 使用较低延迟，但不是立即销毁
            // 可以调整为：
            //Destroy(gameObject, 0f);     // 立即销毁，可能会有一点卡顿
            //Destroy(gameObject, 0.02f);  // 更长的延迟，更平滑但反应稍慢
        }
    }
}