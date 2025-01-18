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
        private float force = 1f;
        
        private Vector2 _dir = Vector2.up;
        private Vector2 _leftLimit = new Vector2(-1, 0);
        private Vector2 _rightLimit = new Vector2(1, 0);
        private int isRotateRight = 0;

        private BubbleEntity _curBubbleEntity;
        private BubbleEntity _nextBubbleEntity;
        public int plusNum = 0;

        private void Awake()
        {
            //资源加载之后修改
            ResManager.GetInstance().LoadAsync<GameObject>("Bubble/" + name, shooter =>
            {
                
            });
            Register();
            InputManager.GetInstance().StartOrStopCheck(true);
        }

        private void Start()
        {
            BubbleManager.GetInstance().CreatBubbleByType(ShooterManager.GetInstance().nextType(), this.transform,
                obj =>
                {
                    _curBubbleEntity = obj;
                }); 
            BubbleManager.GetInstance().CreatBubbleByType(ShooterManager.GetInstance().nextType(), this.transform,
                obj =>
                {
                    _nextBubbleEntity = obj;
                });
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
                    isRotateRight = -1;
                    break;
                case KeyCode.D:
                    isRotateRight = 1;
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
            Vector2 v = _dir;
            float s = isRotateRight * rotateSpeed * Time.deltaTime;
            v.x = _dir.x * Mathf.Cos(s) - _dir.y * Mathf.Sin(s);
            v.y = _dir.x * Mathf.Sin(s) + _dir.y * Mathf.Cos(s);
            _dir = v;
        }

        private void Shooter()
        {
            _curBubbleEntity.AddForce(_dir, force);
            FillNextBubble();
        }

        private void FillNextBubble()
        {
            _curBubbleEntity = _nextBubbleEntity;
            BubbleManager.GetInstance().CreatBubbleByType(ShooterManager.GetInstance().nextType(), this.transform,
                obj =>
                {
                    _nextBubbleEntity = obj;
                });
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
