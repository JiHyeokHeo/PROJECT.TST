using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TST
{
    public class Drone : MonoBehaviour
    {
        public GameObject owner;
        public Target target;

        public Transform droneTransform;

        // 뭐 접히고 날꺼 구현할꺼면 쓰고 아님 말고..
        public GameObject leftWing;
        public GameObject rightWing;

        public Rigidbody droneBulletPrefab;
        public Transform gunFirePoint;
        public Transform missileFirePoint;
        public Rigidbody missilePrefab;

        private Action changeMovement;
        private bool isChangingState = false;
        private bool isShowing = false;

        public bool IsShowing
        {
            get => isShowing;
            set
            {
                if (isChangingState)
                    return;

                isShowing = value;
                if (isShowing)
                    gameObject.SetActive(true);

                // 여기서 일단 다 끔
                isPlayed = false;
                isFiring = false;
                changeMovement = isShowing ? ShowMovement : HideMovement;
                isChangingState = true;
            }
        }

        private void Awake()
        {
            //currentMissileAmmo = clipMissileAmmo;
            //currentAmmo = clipAmmo;
            interactionRange = 10.0f;
        }

        private void Start()
        {
            droneTransform = GetComponent<Transform>();
            transform.position = endPoint.position;
        }

        bool isFiring = false;
        private void Update()
        {
            if (isChangingState)
                changeMovement?.Invoke();

            if (owner == null)
                return;

            if (!isChangingState)
            {
                // 어차피 먼저 움직이고 쏠때 rotation을 다시 변경을 해주기 때문에 문제 없을듯함 순서 바뀌면 X
                FollowOwner();
                Fire();
                Reload();
            }
        }

        public enum EDroneSkillType
        {
            Missile,
            Gun,
            End,
        }

        [Serializable]
        public class DroneSkillData
        {
            public EDroneSkillType skillType;
            public float currentAmmo;
            public float clipAmmo;
            public float fireRate;
            public float reloadNeededTime;
            [ReadOnly] 
            public float reloadElapsedTime;
            [ReadOnly]
            public float lastFireRate;
        }

        public SerializableWrapDictionary<EDroneSkillType, DroneSkillData> skillDataList = new SerializableWrapDictionary<EDroneSkillType, DroneSkillData>();

        // 이것도 뭐 데이터에 넣으려면 넣자..
        public float missileLifeTime = 5f;
        public float bulletLifeTime = 5f;
        public float bulletMoveForce = 1f;

        private Vector3 targetLookDir;
        const float initRotationDiff = 40.0f;

        public void Fire()
        {
            if (target == null)
                return;

            isFiring = true;
            // 미사일 먼저 쏘고 미사일 쐈으면 return
            // 미사일 쿨이면 스킵 때리고 Fire 버전 시작
            // Fire도 쿨이면 그냥 스킵 됨
            if (skillDataList[EDroneSkillType.Missile].currentAmmo > 0 && Time.time - skillDataList[EDroneSkillType.Missile].lastFireRate >= skillDataList[EDroneSkillType.Missile].fireRate)
            {
                for (int i =1; i <= 3; i++)
                {
                    skillDataList[EDroneSkillType.Missile].currentAmmo--;
                    Rigidbody missile = GameObject.Instantiate(missilePrefab, missileFirePoint.position, missileFirePoint.rotation);
                    missile.gameObject.SetActive(true);

                    if (missile.TryGetComponent(out Missile missileComponent))
                    {
                        missileComponent.SetTarget(target);
                        missileComponent.SetInitRotation(new Vector3(-40.0f, -60.0f + (initRotationDiff * i), 0.0f));
                    }
                    Destroy(missile.gameObject, missileLifeTime);

                }
                // 미사일은 굳이 뭐 방햑백터 같은거 설정 해줄 필요 없음 Homing Setting Script(Missile Script 참고)
                skillDataList[EDroneSkillType.Missile].lastFireRate = Time.time;

                // 추후 이펙트 추가
                //var effect = EffectManager.Instance.SpawnEffect(EffectType.Muzzle_1);
                //effect.transform.SetPositionAndRotation(missileFirePoint.position, missileFirePoint.rotation);
                return;
            }

            //// 총알 1번
            //if (skillDataList[EDroneSkillType.Gun].currentAmmo > 0 && Time.time - skillDataList[EDroneSkillType.Gun].lastFireRate >= skillDataList[EDroneSkillType.Gun].fireRate)
            //{
            //    skillDataList[EDroneSkillType.Gun].currentAmmo--;
            //    // 총알은 target 목표에서부터 bullet 위치를 빼서 작업 해줘야 할듯 싶음
            //    skillDataList[EDroneSkillType.Gun].lastFireRate = Time.time;
            //    Rigidbody bullet = GameObject.Instantiate(droneBulletPrefab, gunFirePoint.position, gunFirePoint.rotation);
            //    bullet.gameObject.SetActive(true);

            //    targetLookDir = target.transform.position - bullet.transform.position;
            //    bullet.AddForce(targetLookDir * bulletMoveForce, ForceMode.Impulse);

            //    Destroy(bullet.gameObject, bulletLifeTime);
            //    return;
            //}

            // 타겟을 갖고 있으면 강제 리로드
            Reload(true);
        }
        
        public void Reload(bool forceReload = false)
        {
            if (target == null && forceReload == false)
                return;

            for (EDroneSkillType type = 0; type < EDroneSkillType.End; type++)
                ReloadCoolDownCheck(skillDataList[type]);
            // 하지만 타겟이 없을 때에만 리로드 하도록, Fire 중 리로드 강제 설정

            isFiring = false;
        }

        // 스킬 늘어나면 그냥 enum으로 바꿉시다 
        public bool ReloadCoolDownCheck(DroneSkillData data)
        {
            data.reloadElapsedTime += Time.deltaTime;
            
            if (data.reloadElapsedTime >= data.reloadNeededTime)
            {
                data.reloadElapsedTime = 0;
                data.currentAmmo = data.clipAmmo;
            }

            return true;
        }

        public float interactionRange = 10.0f;
        private List<Target> currentTargetable = new List<Target>();
        private void FixedUpdate()
        {
            // 타겟이 가능한 친구 일단 넣어두고
            Collider[] overlappedObjects = Physics.OverlapSphere(transform.position, interactionRange);
            for (int i = 0; i < overlappedObjects.Length; i++)
            {
                if (overlappedObjects[i].TryGetComponent(out Target targetable))
                {
                    if (false == currentTargetable.Contains(targetable))
                    {
                        currentTargetable.Add(targetable);
                    }
                }

                
            }

            if (target != null)
            {
                float sqrDistance = Vector3.SqrMagnitude(transform.position - target.transform.position);
                if (sqrDistance >= interactionRange * interactionRange)
                {
                    target = null;
                }
            }

            // 최근에 들어간 타겟들 중 하나 뽑자
            if (target == null)
            {
                // 혹시라도 currentTargetable 멤버중 죽은 애가 있을 수도 있으니 delete 시켜주자

                for (int i = currentTargetable.Count - 1; i >= 0; i--)
                {
                    if (currentTargetable[i] == null || currentTargetable[i].gameObject.activeSelf == false)
                        currentTargetable.RemoveAt(i);
                }

                if (currentTargetable.Count > 0)
                {
                    // 인덱스는 -1까지만 하지만 유니티 range는 0이상 targetCount 미만으로 설정 되어 있음 [이상 )미만
                    target = currentTargetable[0];
                }
            }
        }

        bool isPlayed = false;
        private void ShowMovement()
        {
            if (isChangingState == false)
                return;

            if (isPlayed == false)
            {
                StartParabolicMovement(endPoint.position, startPoint.position);
                isPlayed = true;
            }
        }

        private void HideMovement()
        {
            if (isChangingState == false)
                return;

            if (isPlayed == false)
            {
                StartParabolicMovement(startPoint.position, endPoint.position, true);
                isPlayed = true;
            }
        }

        public Transform startPoint;
        public Transform endPoint;
        public float height = 1f;    // 포물선 높이
        public float duration = 1f;  // 이동 시간

        public void StartParabolicMovement(Vector3 startPosition, Vector3 endPosition, bool isHiding = false)
        {
            Vector3 peak = (startPosition + endPosition) / 2 + Vector3.up * height; // 정점 위치

            transform.DOKill();
            // 포물선 애니메이션
            transform.DOPath(new Vector3[] { startPosition, peak, endPosition }, duration, PathType.CatmullRom)
                     .SetEase(Ease.OutQuad)
                     .OnComplete(() =>
                     {
                         isChangingState = false;
                         if (isHiding)
                             gameObject.SetActive(false);
                     });
                    
        }

        public void SetOwner(GameObject owner)
        {
            this.owner = owner;
        }


        public float changeInterval = 2.0f; // 랜덤 회전 목표 변경 간격
        private Quaternion targetRotation; // 목표 회전 값
        private float timeElapsed = 0.0f; // 마지막 랜덤 변경 이후 경과 시간
        Vector3 targetPosition;
        private void FollowOwner()
        {
            if (owner == null)
                return;

            if (isShowing == false)
                return;

            targetPosition = startPoint.position;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10.0f);

            // 주기적으로 랜덤 목표 회전 값 업데이트
            timeElapsed += Time.deltaTime;
            if (timeElapsed >= changeInterval)
            {
                timeElapsed = 0.0f;

                // 랜덤 목표 회전 값 생성

                if (!isFiring)
                {
                    Vector3 newRotation = new Vector3(Random.Range(-10.0f, 10.0f), Random.Range(-30.0f, 30.0f), 0.0f);
                    targetRotation = Quaternion.LookRotation(owner.transform.forward) * Quaternion.Euler(newRotation);
                }
                else
                {
                    targetRotation = Quaternion.LookRotation(targetLookDir);
                }
            }

            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime / 0.5f);
        }
    }
}
