using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace TST
{
    [Serializable]
    public struct ProjectileProperties
    {
        public Vector3 direction;
        public Vector3 initialPosition;
        public float initialSpeed;
        public float mass;
        public float drag;
    }

    public class BombProjectile : ProjectileBase
    {
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private int pointCount = 30;
        [SerializeField] private float timeStep = 0.1f;

        public Vector3 offSetDireciton;

        private bool isThrown = false;
        private bool hasLanded = false;

        public Action effectInvoke;
        private float elapsedLifeTime = 0.0f; 

        public override void Init(CharacterBase owner)
        {
            this.owner = owner;

            rigid = GetComponent<Rigidbody>();
            if (rigid == null )
                rigid = this.AddComponent<Rigidbody>();

            lifeTime = 3.0f;
            effectInvoke += () =>
            {
                EffectManager.Singleton.SpawnEffect(EffectType.BombEffect, this.transform.position, new Vector3(-90.0f, 0.0f, 0.0f));
            };
        }

        public void SetStartTransform(Transform transform)
        {
            startPosition= transform;
        }

        void Update()
        {
            if (rigid == null)
                return;

            if (hasLanded && rigid.velocity.magnitude < 0.1f)
            {
                rigid.velocity = Vector3.zero;
                rigid.angularVelocity = Vector3.zero;
            }

            if (!this.isThrown)
            {
                DrawTrajectory();
            }

            if (this.isThrown)
            {
                elapsedLifeTime += Time.deltaTime;

                if (elapsedLifeTime >= lifeTime)
                {
                    OnLifeTimeExpired();
                }
            }

        }

        public void OffTrajectory()
        {
            lineRenderer.enabled = false;
        }

        Vector3 CalculateParabolicVelocity(Vector3 startPoint, Vector3 targetPoint, float flightTime = 1.0f, float maxDistance = 10.0f)
        {
            Ray screenCenterRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Debug.DrawRay(screenCenterRay.origin, screenCenterRay.direction * maxDistance, Color.red);
            if (Physics.Raycast(screenCenterRay, out RaycastHit Hitinfo, maxDistance))
            {
                targetPoint = Hitinfo.point;
            }
            else
            {
                targetPoint = Camera.main.transform.position + Camera.main.transform.forward * maxDistance;
            }

            Vector3 toTarget = targetPoint - startPoint;
            Vector3 toTargetXZ = new Vector3(toTarget.x, 0, toTarget.z); // 수평 거리

            if (toTarget.magnitude > maxDistance)
            {
                toTarget = toTarget.normalized * maxDistance;
            }

            float y = toTarget.y; // 수직 방향
            float xz = toTargetXZ.magnitude; // 수평 거리의 길이

            // 수평 속도 구하는 공식
            float vxz = xz / flightTime; // 속도 = 거리 / 시간 

            // 중력이 작용하는 가속 운동
            float vy = y / flightTime + 0.5f * Mathf.Abs(Physics.gravity.y) * flightTime;

            Vector3 result = toTargetXZ.normalized * vxz; // 수평 방향 속도
            result.y = vy; // 수직 방향 속도 추가

            return result;
        }

        public void Throw(Vector3 throwStartPoint, Vector3 targetPosition)
        {
            this.isThrown = true;
            rigid.isKinematic = false;
            lineRenderer.enabled = false;

            Vector3 velocity = CalculateParabolicVelocity(throwStartPoint, targetPosition);
            rigid.velocity = velocity;

            //rigid.AddForce(velocity * rigid.mass, ForceMode.Impulse);
        }

        private void OnLifeTimeExpired()
        {
            Collider[] overlapped = Physics.OverlapSphere(transform.position, 5f);
            List<CharacterBase> registedCharacters = new List<CharacterBase>();
            for (int i = 0; i < overlapped.Length; i++)
            {
                if (overlapped[i].transform.root.TryGetComponent(out CharacterBase character))
                {
                    bool isExist = registedCharacters.Exists(x => x == character);
                    if (!isExist)
                    {
                        registedCharacters.Add(character);
                    }
                }
            }

            foreach (var character in registedCharacters)
            {
                character.ApplyDamage(100f, owner.gameObject);
            }

            effectInvoke?.Invoke();        
            lineRenderer.enabled = false;  
            Destroy(this.gameObject);       
        }

        void OnCollisionEnter(Collision collision)
        {
            if (!hasLanded)
            {
                hasLanded = true;
           
            }
        }

        void DrawTrajectory()
        {
            if (startPosition == null)
                return;

            Vector3 startPos = startPosition.position;
            Vector3 velocity = CalculateParabolicVelocity(startPosition.position, 
                CharacterController.Instance.linkedCharacter.aimingPoint.position);   // 던지는 힘 * 6
            Vector3 gravity = Physics.gravity;

            lineRenderer.positionCount = pointCount;

            for (int i = 0; i < pointCount; i++)
            {
                float t = i * timeStep;
                Vector3 point = startPos + velocity * t + 0.5f * gravity * t * t;
                lineRenderer.SetPosition(i, point);
            }
        }
    }
}
