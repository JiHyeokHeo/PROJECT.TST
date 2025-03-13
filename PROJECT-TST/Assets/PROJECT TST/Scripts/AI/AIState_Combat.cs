using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TST
{
    // Sample
    enum EAimingLayer
    {
        Character = 10,
    }

    [Serializable]
    public class AIState_Combat : AIStateBase
    {
        private CharacterBase linkedCharacter;

        // 애니메이터 본으로 몸이나 머리 부위 쏘는거 퍼센트로 랜덤하게 쏘면 될듯하다
        private Transform headTransform;
        private Transform gunFirePoint;

        private float attackRange = 100.0f;
        public AIState_Combat(AICharacterController aiController)
        {
            linkedCharacterController = aiController;
            linkedCharacter = linkedCharacterController.LinkedCharacter;
        }

        public override void Enter()
        {
            if (linkedCharacterController.Target != null)
            {
                var playerComponent = linkedCharacterController.Target.GetComponent<CharacterBase>();
                headTransform = playerComponent.animator.GetBoneTransform(HumanBodyBones.Head);
            }

            // 전투 상태 진입에 따른 초기화 작업. // 주총을 들게하다
            //linkedCharacter.ToggleEquipPrimaryWeapon();

            gunFirePoint = linkedCharacter.primaryWeapon.firePoint;

            // 진입 했을 시 길찾기 정보 끊기
            linkedCharacterController.NavAgent.ResetPath();

            // 추후에 enum 으로 관리하는게 좋을듯 // 둘 중 어느게 더 자주 쓰이는지 물어봅시다. 
            //LayerMask mask = LayerMask.GetMask("Monster") | LayerMask.GetMask("Wall") / 이런 기능도 있다고함 이 친구도 비트를 가져오는 역할이지만 Preference는 모르기에
            int mask = (1 << (int)EAimingLayer.Character);
            aimingLayer = mask;
        }

        public override void Exit()
        {
            // 전투 상태 빠져나갈시 작업.
            //linkedCharacter.ToggleEquipPrimaryWeapon();
        }

        public LayerMask aimingLayer;
        private Quaternion targetRotation;
        private float rotationThreshold = 1.0f; // 각도 변경 임계값 (1도)
        public override void Update()
        {
            // 처음에 센서로 체크
            UpdateCheckSensor();

            UpdateCheckFire();
        }

        private void UpdateCheckSensor()
        {
            GameObject aiTarget = linkedCharacterController.Target;

            if (linkedCharacterController.sensor.IsInSight(aiTarget, attackRange) == false)
                linkedCharacterController.SetState(new AIState_Move(linkedCharacterController));
        }

        private void UpdateCheckFire()
        {
            // 총없으면 그냥 리턴 때려버리고 or 총기 해제한 상태면 업데이트 스킵
            if (linkedCharacter.primaryWeapon == null || headTransform == null || linkedCharacter.IsArmed == false)
                return;

            // Ray 시작 위치를 머리 높이로 설정
            Vector3 startRayPos = gunFirePoint.position;

            // 목표물 방향 계산 (정확하게)
            Vector3 shootDir = (headTransform.transform.position - startRayPos).normalized;

            // Debug Ray로 시각화
            Debug.DrawRay(startRayPos, shootDir * 1000.0f, Color.red);
            if (Physics.Raycast(startRayPos, shootDir, out RaycastHit hitInfo, 1000f, aimingLayer, QueryTriggerInteraction.Ignore))
            {
                linkedCharacter.AIShoot();
            }

            targetRotation = Quaternion.LookRotation(shootDir);
            linkedCharacter.primaryWeapon.firePoint.rotation = Quaternion.LookRotation(shootDir);
            float angleDiff = Quaternion.Angle(linkedCharacter.transform.rotation, targetRotation);
            if (angleDiff > rotationThreshold)
            {
                // 쏠 방향으로 머리를 돌려야한다.
                linkedCharacter.transform.rotation = Quaternion.Lerp(linkedCharacter.transform.rotation, targetRotation, Time.deltaTime * 10.0f);
            }
        }
    }
}
