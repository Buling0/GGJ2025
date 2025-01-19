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
        public void CreatBubbleByType(BubbleType bt, Transform root, UnityAction<BubbleEntity> callback)
        {
            BubbleEntity be = null;
            
            ResManager.GetInstance().LoadAsync<GameObject>("BubblePrefabs/" + bt.ToString(), (obj) =>
            {
                be = obj.GetComponent<BubbleEntity>();
                callback(be);
                
                //对泡泡预制体进行一些初始化操作
                obj.transform.SetParent(root);
                obj.transform.localPosition = Vector3.zero;
            });
            //BEList.Add(be);
        }
        
        public BubbleEntity CreateBubbleByBlend(BubbleType bt, Position root)
        {
            //BubbleEntity be = new BubbleEntity(bt, root);
            //BEList.Add(be);
            return null;
        }

        public void DestoryBubbleList(List<BubbleEntity> bubbles)
        {
            /*foreach (var bubble in bubbles)
            {
                bubble.DestorySelf();
            }*/
            int n = bubbles.Count;
            for (int i = n - 1; i >= 0; i--)
            {
                bubbles[i].DestorySelf();
            }
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

