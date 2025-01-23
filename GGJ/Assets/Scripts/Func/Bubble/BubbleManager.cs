using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Bubble
{
    public class BubbleManager : BaseManager<BubbleManager>
    {
        // 添加泡泡预制体引用
        [SerializeField] private GameObject bubblePrefab;
        private Dictionary<BubbleType, GameObject> _bubblePrefabs = new Dictionary<BubbleType, GameObject>();

        public void CreatBubbleByType(BubbleType bt, Transform root, UnityAction<BubbleEntity> callback)
        {
            //20250121
            if (root == null)
            {
                UnityEngine.Debug.Log("Root transform is null!");
                return;
            }
            
            ResManager.GetInstance().LoadAsync<GameObject>("BubblePrefabs/" + bt.ToString(), (obj) =>
            {
                //20250121
                if (obj == null)
                {
                    UnityEngine.Debug.LogError("Failed to load bubble prefab!");
                    return;
                }
                
                BubbleEntity be = obj.GetComponent<BubbleEntity>();
                if (be == null)
                {
                    UnityEngine.Debug.LogError("BubbleEntity component not found!");
                    return;
                }
                
                //对泡泡预制体进行一些初始化操作
                obj.transform.SetParent(root);
                obj.transform.localPosition = Vector3.zero;
                
                // 准备发射状态
                be.PrepareForShoot();
                
                //20250121
                callback?.Invoke(be);
            });
        }
        
        public BubbleEntity CreateBubbleByBlend(BubbleType bubbleType, Transform parent, Vector3 position)
        {
            BubbleEntity bubble = null;
            ResManager.GetInstance().LoadAsync<GameObject>("BubblePrefabs/" + bubbleType.ToString(), (obj) =>
            {
                if (obj != null)
                {
                    // 直接使用加载的对象，不需要再次Instantiate
                    obj.transform.SetParent(parent);
                    obj.transform.position = position;
                    
                    bubble = obj.GetComponent<BubbleEntity>();
                    if (bubble != null)
                    {
                        // 设置物理属性
                        Rigidbody2D rb = bubble.GetComponent<Rigidbody2D>();
                        if (rb != null)
                        {
                            rb.simulated = true;
                            rb.gravityScale = -1;
                            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                            rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
                            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
                        }
                        
                        // 触发相应的混合完成事件
                        switch (bubbleType)
                        {
                            case BubbleType.Green:
                                EventManager.GetInstance().EventTrigger<BubbleType>("BlendDone", BubbleType.Blue);
                                EventManager.GetInstance().EventTrigger<BubbleType>("BlendDone", BubbleType.Yellow);
                                break;
                            case BubbleType.Orange:
                                EventManager.GetInstance().EventTrigger<BubbleType>("BlendDone", BubbleType.Red);
                                EventManager.GetInstance().EventTrigger<BubbleType>("BlendDone", BubbleType.Yellow);
                                break;
                            case BubbleType.Purple:
                                EventManager.GetInstance().EventTrigger<BubbleType>("BlendDone", BubbleType.Blue);
                                EventManager.GetInstance().EventTrigger<BubbleType>("BlendDone", BubbleType.Red);
                                break;
                        }
                    }
                }
                else
                {
                    Debug.LogError($"Failed to load bubble prefab for type: {bubbleType}");
                }
            });
            
            return bubble;
        }

        public void DestoryBubbleList(List<BubbleEntity> bubbles)
        {
            if (bubbles == null || bubbles.Count == 0) return;
            
            BubbleType bt = bubbles[0].GetBubbleType();
            
            // 使用协程批量处理销毁
            MonoManager.GetInstance().StartCoroutine(BatchDestroy(bubbles));
            
            EventManager.GetInstance().EventTrigger<BubbleType>("EliminateDone", bt);
        }

        private IEnumerator BatchDestroy(List<BubbleEntity> bubbles)
        {
            const int batchSize = 4;  // 每帧处理4个泡泡
            // 可以调整为：
            // const int batchSize = 2;  // 每帧处理更少，更平滑但更慢
            // const int batchSize = 5;  // 每帧处理更多，更快但可能不够平滑
            for (int i = 0; i < bubbles.Count; i += batchSize)
            {
                for (int j = 0; j < batchSize && i + j < bubbles.Count; j++)
                {
                    if (bubbles[i + j] != null)
                    {
                        bubbles[i + j].DestorySelf();
                    }
                }
                yield return null;  // 等待下一帧
            }
            
            GCManager.GetInstance().StartGC();
        }
    }

    //协程GC防止Low帧
    public class GCManager : BaseManager<GCManager>
    {
        public void StartGC()
        {
            MonoManager.GetInstance().StartCoroutine(ReallyGCAsync());
        }

        private IEnumerator ReallyGCAsync()
        {
            GC.Collect();
            yield return null;
        }
    }
}

