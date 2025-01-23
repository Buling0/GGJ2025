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
        [SerializeField] private float rotateSpeed = 2f;
        [SerializeField] private float force = 500f;

        private Vector2 _dir = Vector2.up;
        private int _isRotateRight = 0;
        public float limitAngle = 1;
        public float curAngle = 0;
        private Ray _ray;

        public Transform tran1;  // 当前泡泡的位置
        public Transform tran2;  // 下一个泡泡的位置
        private BubbleEntity _curBubbleEntity;
        private BubbleEntity _nextBubbleEntity;
        public int plusNum = 0;

        public Transform bubbleRod;

        private bool _canShoot = true; // 添加发射状态标记 20250121

        public Transform shooterAway; // 添加 ShooterAway 的引用

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
            // 延迟一帧再初始化，确保所有管理器都准备好
            StartCoroutine(DelayedInit());
        }

        private IEnumerator DelayedInit()
        {
            yield return null;  // 等待一帧

            try
            {
                if (BubbleManager.GetInstance() != null)
                {
                    // 使用 ShooterManager 来获取下一个泡泡类型
                    BubbleManager.GetInstance().CreatBubbleByType(
                        ShooterManager.GetInstance().nextType(),
                        tran1,
                        (bubble) =>
                        {
                            if (bubble != null)
                            {
                                _curBubbleEntity = bubble;
                                BubbleManager.GetInstance().CreatBubbleByType(
                                    ShooterManager.GetInstance().nextType(),
                                    tran2,
                                    nextBubble =>
                                    {
                                        _nextBubbleEntity = nextBubble;
                                        _ray = new Ray();
                                        _ray.origin = this.transform.position;
                                        _ray.direction = _dir;
                                    }
                                );
                            }
                        });
                }
                else
                {
                    Debug.LogError("BubbleManager instance is null!");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error in BubbleShooter Init: {e.Message}");
            }
        }

        private void Register()
        {
            EventManager.GetInstance().AddEventListener<KeyCode>("Key is Down", CheckDown);
            EventManager.GetInstance().AddEventListener<KeyCode>("Key is Up", CheckUp);
        }

        private void Update()
        { 
            if (_isRotateRight != 0)
            {
                RotateDir();
            }

            DrawDir(_dir);
        }

        private void LateUpdate()
        {
            Rotate();
        }

        private void CheckDown(KeyCode key)
        {
            switch (key)
            {
                case KeyCode.A:
                    _isRotateRight = 1;
                    break;
                case KeyCode.D:
                    _isRotateRight = -1;
                    break;
                case KeyCode.Space:
                    Shooter();
                    break;
                default:
                    _isRotateRight = 0;
                    break;
            }
        }

        private void CheckUp(KeyCode key)
        {
            switch (key)
            {
                case KeyCode.A:
                case KeyCode.D:
                    _isRotateRight = 0;
                    break;
                default:
                    break;
            }
        }

        private void RotateDir()
        {
            float newAngle = curAngle + _isRotateRight * rotateSpeed * Time.deltaTime * 0.1f;
            curAngle = Mathf.Clamp(newAngle, -1.5f, 1.5f);

            Vector2 v = _dir;
            
            v.x = Vector2.up.x * Mathf.Cos(curAngle) - Vector2.up.y * Mathf.Sin(curAngle);
            v.y = Vector2.up.x * Mathf.Sin(curAngle) + Vector2.up.y * Mathf.Cos(curAngle);
            
            _dir = v;
            
            // 更新泡泡杆的旋转
            float angle = Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg;
            bubbleRod.rotation = Quaternion.Euler(0, 0, angle + 90f);
            
            // 确保泡泡杆的位置与射线的起点一致
            bubbleRod.position = _ray.origin;

            // 让 tran1 跟随射线旋转，但不旋转泡泡本身
            if (tran1 != null)
            {
                // 移动 tran1 的位置，但保持泡泡朝向不变
                float offset = 130f;
                tran1.position = _ray.origin + (Vector3)(_dir.normalized * offset);
                
                // 设置 tran1 的旋转为 0，防止泡泡翻转
                tran1.rotation = Quaternion.identity;
                
                // 如果当前有泡泡，确保其保持正确朝向
                if (_curBubbleEntity != null)
                {
                    _curBubbleEntity.transform.rotation = Quaternion.identity;
                }
            }
        }

        private void Rotate()
        {
            //this.transform.rotation = new Vector3(_dir.x, _dir.y, 0);
        }

        private void DrawDir(Vector2 dir)
        {
            _ray.direction = dir;
            Debug.DrawRay(_ray.origin, _ray.direction * 200, Color.green);
        }

        private void Shooter()
        {
            if (!_canShoot || _curBubbleEntity == null) return;
            
            _canShoot = false;
            
            // 发射前将泡泡设为 ShooterAway 的子物体
            if (shooterAway != null)
            {
                _curBubbleEntity.transform.SetParent(shooterAway);
                // 确保发射时泡泡朝向正确
                _curBubbleEntity.transform.rotation = Quaternion.identity;
            }
            
            _curBubbleEntity.AddForce(_dir, force);
            Invoke("FillNextBubble", 0.6f);
        }

        private void FillNextBubble()
        {
            if (_nextBubbleEntity == null) return;
            
            _curBubbleEntity = _nextBubbleEntity;
            _curBubbleEntity.transform.SetParent(tran1);
            _curBubbleEntity.transform.localPosition = Vector3.zero;
            
            BubbleManager.GetInstance().CreatBubbleByType(
                ShooterManager.GetInstance().nextType(), 
                tran2,
                obj => { 
                    _nextBubbleEntity = obj;
                    _canShoot = true;
                }
            );

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