//using System;
//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.AI;
//using UnityEngine.UIElements;

//namespace TST
//{
//    [Serializable]
//    public class AIState_Move : AIStateBase
//    {
//        private CharacterBase linkedCharacter;

//        // 기억시간 5초동안은 추적하고 그 안에 숨거나 하면 추적 실패
//        public float memorableTime = 5f;
//        private float elapsedTime = 0f;

//        // 공격 범위
//        public float attackDistance = 10f;

//        public AIState_Move(AICharacterController aiController)
//        {
//            linkedCharacter = aiController.LinkedCharacter;
//            linkedCharacterController = aiController;
//        }

//        public override void Enter()
//        {

//        }

//        public override void Exit()
//        {
//            // NavMesh 쓰고 있었다면 탈출과 동시에 Path 서칭 취소
//        }

//        public override void Update()
//        {
//            if (linkedCharacterController.NavAgent.pathPending == false && linkedCharacterController.NavAgent.remainingDistance < 0.01f)
//            {
//                FollowTarget();
//            }

//            UpdateMemorialCheck();
//            UpdateCombatMode();
//        }

//        private void UpdateMemorialCheck()
//        {
//            // 만약 센서에 측정된 타겟이 없으면 당장엔 타겟을 지우진 않는다.
//            // 하지만 메모리얼 시간이 넘는 순간 Target을 민다. 그리고 다시 패트롤 상태로 전환
//            if (linkedCharacterController.sensor.Objects.Count == 0)
//            {
//                elapsedTime += Time.deltaTime;
//                if (elapsedTime >= memorableTime)
//                {
//                    linkedCharacterController.Target = null;
//                    elapsedTime = 0f;
//                    linkedCharacterController.SetState(new AIState_Patrol(linkedCharacterController));
//                }
//            }
//        }

//        private void UpdateCombatMode()
//        {
//            // 타겟이 비어있다면 return
//            if (linkedCharacterController.Target == null)
//                return;

//            if (linkedCharacterController.sensor.IsInSight(linkedCharacterController.Target) == false)
//                return;

//            Vector3 targetPosition = linkedCharacterController.Target.transform.position;

//            // 범위 내에 있으면 공격
//            float dist = Vector3.Distance(targetPosition, linkedCharacter.transform.position);
//            if (dist <= attackDistance)
//            {
//                linkedCharacterController.SetState(new AIState_Combat(linkedCharacterController));
//            }
//        }

//        private void FollowTarget()
//        {
//            if (linkedCharacterController.Target == null)
//            {
//                Debug.Log("ai Target Issue");
//                return;
//            }

//            //// 총 빼면서 움직이는거 방지하기 위해서
//            //if (linkedCharacter.IsArmed == false && linkedCharacter.IsArmedCompleted == true)
//            //{
//            //    linkedCharacterController.NavAgent.ResetPath();
//            //    return;
//            //}
//            //// 반대 버전
//            //if (linkedCharacter.IsArmed == true && linkedCharacter.IsArmedCompleted == false)
//            //{
//            //    linkedCharacterController.NavAgent.ResetPath();
//            //    return;
//            //}

//            // 현재 캐릭터 위치를 기준으로 랜덤한 위치 계산
//            Vector3 aiPosition = linkedCharacterController.Target.transform.position;

//            // 목표 위치 설정
//            linkedCharacterController.SetDestination(aiPosition);
//        }
    
//}
//}
