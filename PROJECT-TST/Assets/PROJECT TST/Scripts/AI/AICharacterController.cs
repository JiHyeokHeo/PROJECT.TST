using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace TST
{
    public class AICharacterController : MonoBehaviour
    {
        public bool isItemGenerated = false;
        public Transform dropItemPositon;
        public CharacterBase LinkedCharacter => characterBase;
        public NavMeshAgent NavAgent
        {
            get
            {
                if (navAgent == null)
                {
                    Debug.Log("navAgent is Null");
                }
                return navAgent;
            }
        }

        public GameObject Target
        {
            get
            {
                if (target == null)
                {
                    Debug.Log("target is Null");
                    return null;
                }

                return target;
            }

            set { target = value; }
        }

        [SerializeReference]
        public AIStateBase currentState;
        public AiSensor sensor;

        private GameObject target;
        private CharacterBase characterBase;
        private NavMeshAgent navAgent;

        public Vector3 AISpawnPosition;

        public bool isTargetPositionOn = false;
        private Vector3 targetPosition;

        public float elapsedSearchingFailTime = 0f;
        public float patrolFailedTime = 3.0f;
        public Transform[] patrolPoints;

        private void Awake()
        {
            characterBase = GetComponent<CharacterBase>();
            navAgent = GetComponent<NavMeshAgent>();
            AISpawnPosition = characterBase.transform.position;

            navAgent.updatePosition = false;
            navAgent.updateRotation = false;
        }

        private void Start()
        {
            // 상태 객체를 미리 생성해 둠
            currentState = new AIState_Patrol(this);
            characterBase.ToggleEquipPrimaryWeapon();
            //characterBase.OnDamaged += (target) => SetState(new AIState_Combat(this));
            //characterBase.OnDamaged += (target) => SetTarget(target);

            //// Sensor 스크립트 안에 있다면 Combat 스테이트로 진입
            //// 공격 범위 밖에 있다가 다시 탐지 범위에 들어가게 되도 공격모드 진입 
            ////characterBase.OnDetect += (target) => SetState<GameObject>(new AIState_Move(characterBase, agent),
            ////    beforeEnterEvent: (t) => SetTarget(target));

            //characterBase.OnDetect += (target) => SetState(new AIState_Move(this));
            //characterBase.OnDetect += (target) => SetTarget(target);


            //characterBase.OnCombatDetect += (target) => SetState(new AIState_Combat(this));
            //characterBase.OnCombatDetect += (target) => SetTarget(target); 

            // Sensor 스크립트 탐지 범위 바깥으로 빠지면 Idle 상태로 진입
            //characterBase.OnIdle += (target) => SetState(new AIState_Idle(this));

            // 결론 처음엔 Patrol 진입 하지만 센서로 인해 Combat or Idle 상태로 진입 // Idle 상태에서 특정 시간이 되면 다시 Patrol 진입
        }

        // 스크립트를 끄고 키는 것으로 조절을 해볼까?..
        private void OnEnable()
        {
            isItemGenerated = false;
        }

        private void Update()
        {
            AIDropCheck();

            currentState.Update();

            // NavAgent의 다음 위치 값을, 현재 위치로 설정
            navAgent.nextPosition = transform.position;

            if (navAgent.pathStatus == NavMeshPathStatus.PathComplete && RemainingDistance() <= navAgent.stoppingDistance)
            {
                // 도착 했을 때
                characterBase.Move(Vector2.zero, transform.eulerAngles.y);
                isTargetPositionOn = false;
            }
            else // 아직 도착 XXX
            {
                if (navAgent.hasPath) // 경로가 있는 경우 => navAgent가 목적지로 이동 중인 경우
                {
                    Vector3 moveDir = navAgent.steeringTarget - transform.position;
                    if (moveDir.sqrMagnitude > 0.01f)
                    {
                        Vector3 moveDirection = moveDir.normalized;
                        Vector2 input = new Vector2(moveDirection.x, moveDirection.z);

                        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                        float angleDiff = Quaternion.Angle(transform.rotation, targetRotation);
                        if (angleDiff > 1.0f)
                        {
                            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10.0f);
                        }

                        if (characterBase.IsArmed)
                            characterBase.Move(new Vector2(0.0f, 1.0f), 0);
                        else
                            characterBase.Move(input, 0);
                    }
                }
                else // 경로가 없는 경우 => NavAgent가 목적지로 이동하지 않는 경우엔 스탑
                {
                    characterBase.Move(Vector2.zero, 0);
                }
            }

            Debug.Log($"{currentState}");
        }

        public float RemainingDistance()
        {
            if (!navAgent.isOnNavMesh)
                return float.MaxValue;
            if (navAgent.pathPending)
                return float.MaxValue;

            return navAgent.remainingDistance;
        }

        public void SetState(AIStateBase newState)
        {
            SetState<GameObject>(newState);
        }

        public void SetState<T>(AIStateBase newState, Action<T> beforeEnterEvent = null, T beforeEnterParam = default)
        {
            if (currentState == newState)
                return;

            currentState.Exit();
            currentState = newState;
            beforeEnterEvent?.Invoke(beforeEnterParam);
            currentState.Enter();
        }

        public void SetDestination(Vector3 destination)
        {
            if (isTargetPositionOn == false)
            {
                navAgent.SetDestination(destination);
            }

            // 중복 설정 방지
            isTargetPositionOn = true;
        }

        private void AIDropCheck()
        {
            Assert.IsFalse(isItemGenerated, "This AI is Already Generate Item ! Need to Reset");

            if (LinkedCharacter.CurrentHp <= 0)
            {
                isItemGenerated = true;

                // 아이템 움직임 이펙트
                GameManager.Instance.GenerateItem(transform.position);

                GetComponent<AICharacterController>().enabled = false;
            }
        }

#if UNITY_EDITOR
        //private void OnDrawGizmosSelected()

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            if (patrolPoints != null && patrolPoints.Length > 1)
            {
                for (int i = 0; i < patrolPoints.Length; i++)
                {
                    if (patrolPoints[i] != null)
                    {
                        Gizmos.DrawSphere(patrolPoints[i].position, 0.3f);

                        if (i < patrolPoints.Length - 1 && patrolPoints[i + 1] != null)
                        {
                            Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
                        }
                    }
                }
            }
        }
#endif
    }
}
