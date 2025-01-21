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
        private bool isBeLiminate = false;
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
            int i = Dic[bubbleType].Count - 1;
            for (; i >= 0; i--)
            {
                var be = list[i];
                be.isDestorying = true;
                IEnumerable<BubbleEntity> l = list.Union(be.AdjoinBubbleDic.BeEliminate(bubbleType));
                foreach (var bubble in l)
                {
                    if (!list.Contains(bubble))
                    {
                        list.Add(bubble);
                    }
                }
            }

            num = list.Count;

            ScoreManager.GetInstance().PlusScore(bubbleType, num);
            BubbleManager.GetInstance().DestoryBubbleList(list);
        }

        public List<BubbleEntity> BeEliminate(BubbleType bubbleType)
        {
            if (!Dic.ContainsKey(bubbleType))
            {
                isBeLiminate = true;
                return null;
            }
            else if (Dic[bubbleType].Count <= 1 || isBeLiminate)
            {
                return Dic[bubbleType];
            }
            else
            {
                List<BubbleEntity> list = Dic[bubbleType];
                int i = Dic[bubbleType].Count - 1;
                for (; i >= 0; i--)
                {
                    var be = list[i];
                    if (!be.isDestorying)
                    {
                        be.isDestorying = true;
                        IEnumerable<BubbleEntity> l = list.Union(be.AdjoinBubbleDic.BeEliminate(bubbleType));
                        foreach (var bubble in l)
                        {
                            if (!list.Contains(bubble))
                            {
                                list.Add(bubble);
                            }
                        }
                    }
                }

                isBeLiminate = true;

                return list;
            }
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

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
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

        //传入发射方向单位向量，fv是力的大小
        public void AddForce(Vector2 dir, float fv)
        {
            if (_rigidbody2D == null) return;//20250121
            _rigidbody2D.AddForce(dir * fv, ForceMode2D.Impulse);
        }

        //碰撞开始 把碰撞物体加入字典
        private void OnCollisionEnter2D(Collision2D bubble)
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

            bool canBlend;
            CheckCanBlend(bubbleEntity, out canBlend);
            if (canBlend && !isBlending)
            {
                Debug.Log("blend");
                isBlending = true;
                bubbleEntity.isBlending = true;
                EventManager.GetInstance().EventTrigger<BubbleEntity>("Blend", bubbleEntity);
                return;
            }

            AdjoinBubbleDic.Add(bubbleEntity);
            if (CheckCanEliminate())
            {
                EventManager.GetInstance().EventTrigger<BubbleEntity>("Eliminate", bubbleEntity);
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

        //消除检测
        private bool CheckCanEliminate()
        {
            if (AdjoinBubbleDic.CheckTypeNum(_bubbleType) >= 2)
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

        //进行消除
        public void Eliminate(BubbleEntity bubbleEntity)
        {
            if (bubbleEntity == this && !isDestorying)
            {
                isDestorying = true;
                int num;
                //AdjoinBubbleDic.Add(this);
                AdjoinBubbleDic.EliminateByType(bubbleEntity.GetBubbleType(), out num);
                Debug.Log("消除了" + num + "个");
                if (num == 0)
                {
                    isDestorying = false;
                    return;
                }

                DestorySelf();
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
            if (gameObject == null) return;//20250121
            
            gameObject.SetActive(false);
            AdjoinBubbleDic.Clear();
            UnRegister();
            
            // 确保在销毁前清理引用20250121
            _rigidbody2D = null;
            Destroy(this.gameObject);
        }
    }
}