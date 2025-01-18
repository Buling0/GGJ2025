using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

namespace Bubble
{
    public class BubbleShooter : MonoBehaviour
    {
        [SerializeField]
        private float rotateSpeed = 1f;
        [SerializeField]
        private float force = 500f;
        
        private Vector2 _dir = Vector2.up;
        private int isRotateRight = 0;
        public float limitAngle = 80;

        public Transform tran1;
        public Transform tran2;
        private BubbleEntity _curBubbleEntity;
        private BubbleEntity _nextBubbleEntity;
        public int plusNum = 0;

        private void Awake()
        {
            //资源加载之后修改
            /*ResManager.GetInstance().LoadAsync<GameObject>("Bubble/" + name, shooter =>
            {
                
            });*/
            Register();
            InputManager.GetInstance().StartOrStopCheck(true);
        }

        private void Start()
        {
            BubbleManager.GetInstance().CreatBubbleByType(ShooterManager.GetInstance().nextType(), tran1,
                obj =>
                {
                    _curBubbleEntity = obj;
                }); 
            BubbleManager.GetInstance().CreatBubbleByType(ShooterManager.GetInstance().nextType(), tran2,
                obj =>
                {
                    _nextBubbleEntity = obj;
                });
            Debug.Log("Start");
        }

        private void Register()
        {
            EventManager.GetInstance().AddEventListener<KeyCode>("Key is Down", CheckDown);
            EventManager.GetInstance().AddEventListener<KeyCode>("Key is Up", CheckUp);
        }

        private void Update()
        {
            if (_curBubbleEntity == null)
            {
                Debug.Log("1");
            }
            
            if (isRotateRight != 0)
            {
                RotateDir();
            }
        }

        private void CheckDown(KeyCode key)
        {
            switch (key)
            {
                case KeyCode.A:
                    isRotateRight = 1;
                    break;
                case KeyCode.D:
                    isRotateRight = -1;
                    break;
                case KeyCode.Space:
                    Shooter();
                    break;
                default:
                    isRotateRight = 0;
                    break;
            }
        }

        private void CheckUp(KeyCode key)
        {
            switch (key)
            {
                case KeyCode.A:
                case KeyCode.D:
                    isRotateRight = 0;
                    break;
                default:
                    break;
            }
        }

        private void RotateDir()
        {
            float s = isRotateRight * rotateSpeed * Time.deltaTime;
            if (s >= limitAngle)
            {
                s = limitAngle;
            }
            else if(s <= -limitAngle)
            {
                s = -limitAngle;
            }
            _dir.x = Vector2.up.x * Mathf.Cos(s) - Vector2.up.y * Mathf.Sin(s);
            _dir.y = Vector2.up.x * Mathf.Sin(s) + Vector2.up.y * Mathf.Cos(s);
        }

        private void Shooter()
        {
            _curBubbleEntity.AddForce(_dir, force);
            Invoke("FillNextBubble", 0.6f);
        }

        private void FillNextBubble()
        {
            _curBubbleEntity = _nextBubbleEntity;
            _curBubbleEntity.transform.SetParent(tran1);
            _curBubbleEntity.transform.localPosition = Vector3.zero;
            BubbleManager.GetInstance().CreatBubbleByType(ShooterManager.GetInstance().nextType(), tran2,
                obj =>
                {
                    _nextBubbleEntity = obj;
                });
            
            StartCheck();
        }

        private void StartCheck()
        {
            InputManager.GetInstance().StartOrStopCheck(true);
        }

        private void UnRegister()
        {
            EventManager.GetInstance().RemoveEventListener<KeyCode>("Key is Down", CheckDown);
            EventManager.GetInstance().RemoveEventListener<KeyCode>("Key is Up", CheckUp);
        }

        private void OnDestroy()
        {
            UnRegister();
        }
    }

    public class ShooterManager : BaseManager<ShooterManager>
    {
        public BubbleType nextType()
        {
            return (BubbleType)Random.Range(0, 4);
        } 
    }
}
