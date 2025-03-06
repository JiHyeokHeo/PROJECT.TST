using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace TST
{
    [Serializable]
    public class AIState_Idle : AIStateBase
    {
        private CharacterBase linkedCharacter;
        private float idleDuration = 5.0f; // 대기 시간
        private float idleStartTime;

        private Vector3 spawnPosition;

        public AIState_Idle(AICharacterController aiController)
        {
            linkedCharacterController = aiController;
            linkedCharacter = aiController.LinkedCharacter;
        }

        public override void Enter()
        {
            // 현재 위치를 기준으로 복귀 좌표 설정
            spawnPosition = linkedCharacter.transform.position;
            idleStartTime = Time.time;


            linkedCharacterController.SetDestination(spawnPosition);
        }

        public override void Exit()
        {

        }

        public override void Update()
        {
            // 대기 시간이 경과했으면 Patrol 상태로 전환
            if (Time.time - idleStartTime >= idleDuration)
            {
                linkedCharacterController.SetState(new AIState_Patrol(linkedCharacterController));
            }
        }
    }
}
