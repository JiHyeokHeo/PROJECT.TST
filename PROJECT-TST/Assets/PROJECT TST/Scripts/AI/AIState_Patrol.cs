using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TST
{
    [Serializable]
    public class AIState_Patrol : AIStateBase
    {
        private CharacterBase linkedCharacter;

        public float patrolInterval = 4.0f; // 순찰 시간 간격
        private float lastPatrolTime;

        public float patrolRange = 5.0f; // 순찰 범위
        public float patrolMaxRange = 10.0f; // 순찰 범위
        private Vector3 targetPosition;

        public bool returnToInitPos = true; // 처음에 자기 자리로 돌아가도록 명령

        public AIState_Patrol(AICharacterController aiController)
        {
            linkedCharacterController = aiController;
            linkedCharacter = linkedCharacterController.LinkedCharacter;
        }

        public override void Enter()
        {
            // 패트롤 상태로 갔으면 일단 자기 원래 있던 위치로 이동 시키자
            UpdatePatrolDestination();
        }

        public override void Exit()
        {
            // Exit 시 필요한 로직이 있을 경우 추가
            linkedCharacterController.NavAgent.ResetPath();
        }

        public override void Update()
        {
            if (Time.time - lastPatrolTime > patrolInterval && linkedCharacterController.isTargetPositionOn == false)
            {
                UpdatePatrolDestination();
            }

            //// 하지만 너무 멀리 떨어져있으면 그냥 다시 제자리로 돌아가도록 명령
            //float dist = Vector3.SqrMagnitude(linkedCharacter.transform.position - linkedCharacterController.AISpawnPosition);
            //if (dist >= patrolMaxRange * patrolMaxRange)
            //{
            //    returnToInitPos = true;
            //}
            
            //if (dist < 1.0f)
            //{
            //    returnToInitPos = false;
            //}


            if (UpdateFindTarget())
            {
                linkedCharacterController.SetState(new AIState_Move(linkedCharacterController));
            }
        }

        private bool UpdateFindTarget()
        {
            if (linkedCharacterController.sensor.Objects.Count > 0 )
            {
                linkedCharacterController.Target = linkedCharacterController.sensor.Objects[0];
                return true;
            }

            return false;
        }

        private int currentIndex = -1;
        // bool 형 returnToInitPos 에 의해 스폰 위치로 갈지 랜덤 위치로 움직일지 결정
        private void UpdatePatrolDestination()
        {
            lastPatrolTime = Time.time;

            if (linkedCharacterController.patrolPoints.Length > 0)
            {
                // 넘어서면 0 
                currentIndex = currentIndex + 1 >= linkedCharacterController.patrolPoints.Length ? 0 : currentIndex + 1;

                if (linkedCharacterController.patrolPoints[currentIndex] == null)
                    return;

                targetPosition = linkedCharacterController.patrolPoints[currentIndex].position;
            }

            // 목표 위치 설정
            linkedCharacterController.SetDestination(targetPosition);
        }
    }
}
