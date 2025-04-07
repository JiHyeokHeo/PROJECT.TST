using System;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    enum EAimingLayer
    {
        Character = 10,
    }

    [Serializable]
    public class AIState_Combat : AIStateBase
    {
        private CharacterBase linkedCharacter;

        private Dictionary<HumanBodyBones, float> boneTargetChances = new Dictionary<HumanBodyBones, float>
        {
            { HumanBodyBones.Head, 0.1f },
            { HumanBodyBones.Hips, 0.3f },
            { HumanBodyBones.RightLowerLeg, 0.15f },
            { HumanBodyBones.LeftLowerLeg, 0.15f },
            { HumanBodyBones.RightLowerArm, 0.15f },
            { HumanBodyBones.LeftLowerArm, 0.15f }
        };

        private Transform gunFirePoint;
        private float attackRange = 10.0f;

        private Transform currentTargetBone;
        private float targetBoneLastSetting = 0f;
        private float targetBoneUpdateCooldown = 1.0f;

        public AIState_Combat(AICharacterController aiController)
        {
            linkedCharacterController = aiController;
            linkedCharacter = linkedCharacterController.LinkedCharacter;
        }

        public override void Enter()
        {
            gunFirePoint = linkedCharacter.primaryWeapon.firePoint;
            linkedCharacterController.NavAgent.ResetPath();

            int mask = 1 << (int)EAimingLayer.Character;
            aimingLayer = mask;

#if UNITY_EDITOR
            float totalChance = 0f;
            foreach (var chance in boneTargetChances.Values)
                totalChance += chance;

            if (totalChance < 0.99f || totalChance > 1.01f)
                Debug.LogWarning($"[AI] boneTargetChances È®·ü ÇÕÀÌ 1.0ÀÌ ¾Æ´Õ´Ï´Ù: {totalChance}");
#endif
        }

        public override void Exit()
        {
            currentTargetBone = null;
        }

        public LayerMask aimingLayer;
        private Quaternion targetRotation;
        private float rotationThreshold = 1.0f;

        public override void Update()
        {
            UpdateCheckSensor();

            // ÄðÅ¸ÀÓ¿¡ µû¶ó º» Å¸°Ù Àç¼³Á¤
            if (Time.time - targetBoneLastSetting >= targetBoneUpdateCooldown)
            {
                targetBoneLastSetting = Time.time;
                currentTargetBone = GetRandomTargetBone();
            }

            UpdateCheckFire();
        }

        private void UpdateCheckSensor()
        {
            GameObject aiTarget = linkedCharacterController.Target;

            if (linkedCharacterController.LinkedCharacter.CurrentHp <= 0)
                linkedCharacterController.SetState(new AIState_Move(linkedCharacterController));

            if (linkedCharacterController.sensor.IsInSight(aiTarget, attackRange) == false)
                linkedCharacterController.SetState(new AIState_Move(linkedCharacterController));
        }

        private void UpdateCheckFire()
        {
            if (linkedCharacter.primaryWeapon == null || linkedCharacter.IsArmed == false)
                return;

            if (currentTargetBone == null)
                return;

            Vector3 startRayPos = gunFirePoint.position;
            Vector3 shootDir = (currentTargetBone.position - startRayPos).normalized;

            Debug.DrawRay(startRayPos, shootDir * 1000.0f, Color.red);
            if (Physics.Raycast(startRayPos, shootDir, out RaycastHit hitInfo, 1000f, aimingLayer, QueryTriggerInteraction.Ignore))
            {
                linkedCharacter.AIShoot();
            }

            targetRotation = Quaternion.LookRotation(shootDir);
            linkedCharacter.primaryWeapon.firePoint.rotation = targetRotation;

            float angleDiff = Quaternion.Angle(linkedCharacter.transform.rotation, targetRotation);
            if (angleDiff > rotationThreshold)
            {
                linkedCharacter.transform.rotation 
                    = Quaternion.Lerp(linkedCharacter.transform.rotation, targetRotation, Time.deltaTime * 10.0f);
            }
        }

        private Transform GetRandomTargetBone()
        {
            var targetGameObject = linkedCharacterController.Target;
            if (targetGameObject == null)
                return null;

            var characterBase = targetGameObject.GetComponent<CharacterBase>();
            if (characterBase == null || characterBase.animator == null)
                return targetGameObject.transform;

            float rand = UnityEngine.Random.value;
            float cumulative = 0f;

            foreach (var pair in boneTargetChances)
            {
                cumulative += pair.Value;
                if (rand < cumulative)
                {
                    var bone = characterBase.animator.GetBoneTransform(pair.Key);
                    if (bone != null)
                        return bone;
                }
            }

            return targetGameObject.transform;
        }
    }
}