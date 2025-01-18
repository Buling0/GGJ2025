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
            return Dic[bubbleType].Count;
        }

        public void EliminateByType(BubbleType bubbleType, out int num)
        {
            List<BubbleEntity> list = Dic[bubbleType];
            foreach (var be in Dic[bubbleType])
            {
                list.Union(be.AdjoinBubbleDic.BeEliminate(bubbleType));
            }
            num = list.Count;
            
            BubbleManager.GetInstance().DestoryBubbleList(list);
        }

        public List<BubbleEntity> BeEliminate(BubbleType bubbleType)
        {
            if (Dic[bubbleType].Count <= 1)
            {
                return null;
            }
            else
            {
                List<BubbleEntity> list = Dic[bubbleType];
                foreach (var be in Dic[bubbleType])
                {
                    list.Union(be.AdjoinBubbleDic.BeEliminate(bubbleType));
                }

                return list;
            }
        }

        public void Remove(BubbleEntity bubbleEntity)
        {
            BubbleType bubbleType = bubbleEntity.GetBubbleType();
            Dic[bubbleType].Remove(bubbleEntity);
            if (Dic[bubbleType].Count == 0)
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
    }


    public class BubbleEntity : MonoBehaviour
    {
        [SerializeField]
        private BubbleType _bubbleType;
        private Rigidbody2D _rigidbody2D;
        private Image _image;
        public BubbleDic AdjoinBubbleDic = new BubbleDic();

        /*public BubbleEntity(BubbleType bt)
        {
            _bubbleType = bt;
        }

        //合成时创建，需要给出创建的位置
        public BubbleEntity(BubbleType bt, RectTransform root)
        {
            _bubbleType = bt;
        }*/

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            Register();
        }

        private void Register()
        {
            EventManager.GetInstance().AddEventListener<BubbleEntity>("Blend", Blend);
            EventManager.GetInstance().AddEventListener<BubbleEntity>("Eliminate", Eliminate);
        }

        public BubbleType GetBubbleType()
        {
            BubbleType bubbleType = _bubbleType;
            return bubbleType;
        }

        //传入发射方向单位向量，fv是力的大小
        public void AddForce(Vector2 dir, float fv)
        {
            Debug.Log("add");
            _rigidbody2D.AddForce(dir * fv, ForceMode2D.Impulse);
        }

        //碰撞开始 把碰撞物体加入字典
        private void OnCollisionEnter(Collision bubble)
        {
            BubbleEntity bubbleEntity = bubble.gameObject.GetComponent<BubbleEntity>();

            bool canBlend;
            CheckCanBlend(bubbleEntity, out canBlend);
            if (canBlend)
            {
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
        private void OnCollisionExit(Collision bubble)
        {
            BubbleEntity bubbleEntity = bubble.gameObject.GetComponent<BubbleEntity>();
            AdjoinBubbleDic.Remove(bubbleEntity);
        }

        //颜色合成检测
        private void CheckCanBlend(BubbleEntity bubbleEntity, out bool canBlend)
        {
            BubbleType bt = bubbleEntity.GetBubbleType();
            if (_bubbleType == BubbleType.White || bt == _bubbleType || bt == BubbleType.White)
            {
                canBlend = false;
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

            canBlend = false;
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
            if (bubbleEntity != this)
            {
                Transform p1 = bubbleEntity.gameObject.transform;
                Transform p2 = this.gameObject.transform;
                /*BubbleManager.GetInstance()
                    .CreateBubbleByBlend(bubbleEntity.GetBubbleType(), (p1.position + p2.position) / 2);*/
            }
        }

        //进行消除
        public void Eliminate(BubbleEntity bubbleEntity)
        {
            if (bubbleEntity == this)
            {
                int num;
                AdjoinBubbleDic.EliminateByType(bubbleEntity.GetBubbleType(), out num);
                ScoreManager.GetInstance().PlusScore(bubbleEntity.GetBubbleType(), num);
            }
        }

        private void UnRegister()
        {
            EventManager.GetInstance().RemoveEventListener<BubbleEntity>("Blend", Blend);
            EventManager.GetInstance().RemoveEventListener<BubbleEntity>("Eliminate", Eliminate);
        }

        public void DestorySelf()
        {
            Destroy(this);
        }

        private void OnDestroy()
        {
            AdjoinBubbleDic.Clear();
            UnRegister();
        }
    }
}