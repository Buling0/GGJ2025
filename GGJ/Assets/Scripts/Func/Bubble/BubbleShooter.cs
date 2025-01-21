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

        public Transform tran1;
        public Transform tran2;
        private BubbleEntity _curBubbleEntity;
        private BubbleEntity _nextBubbleEntity;
        public int plusNum = 0;

        public Transform bubbleRod; // 泡泡杆的Transform引用
        public Transform shooterRoot; // ShooterRoot的Transform引用

        private bool _canShoot = true; // 添加发射状态标记 20250121

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
            BubbleManager.GetInstance().CreatBubbleByType(ShooterManager.GetInstance().nextType(), tran2,
                obj => { _nextBubbleEntity = obj; });
            BubbleManager.GetInstance().CreatBubbleByType(ShooterManager.GetInstance().nextType(), tran1,
                obj => { _curBubbleEntity = obj; });
            _ray = new Ray();
            _ray.origin = this.transform.position;
            _ray.direction = _dir;
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
            // 计算新的角度
            float newAngle = curAngle + _isRotateRight * rotateSpeed * Time.deltaTime * 0.1f;
            
            // 限制角度范围在-1.5到1.5之间
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
            
            // 更新ShooterRoot的位置
            if (shooterRoot != null)
            {
                shooterRoot.position = _ray.origin;
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
            
            _canShoot = false; // 发射后禁止连续发射 20250121
            _curBubbleEntity.AddForce(_dir, force);
            Invoke("FillNextBubble", 0.6f);
        }

        private void FillNextBubble()
        {
            if (_nextBubbleEntity == null) return; //20250121
            
            _curBubbleEntity = _nextBubbleEntity;
            _curBubbleEntity.transform.SetParent(tran1);
            _curBubbleEntity.transform.localPosition = Vector3.zero;
            
            // 创建下一个泡泡 20250121
            BubbleManager.GetInstance().CreatBubbleByType(
                ShooterManager.GetInstance().nextType(), 
                tran2,
                obj => { 
                    _nextBubbleEntity = obj;
                    _canShoot = true; // 新泡泡准备好后才允许发射
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