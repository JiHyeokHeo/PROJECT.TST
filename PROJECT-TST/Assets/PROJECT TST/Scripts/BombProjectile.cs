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

        protected override void Init()
        {
            rigid = GetComponent<Rigidbody>();
            if (rigid == null )
                rigid = this.AddComponent<Rigidbody>();

            lifeTime = 3.0f;
        }

        public void SetStartTransform(Transform transform)
        {
            startPosition= transform;
        }

        void Update()
        {
            if (hasLanded && rigid.velocity.magnitude < 0.1f)
            {
                rigid.velocity = Vector3.zero;
                rigid.angularVelocity = Vector3.zero;
            }

            if (!this.isThrown)
            {
                DrawTrajectory();
            }
        }

        Vector3 CalculateParabolicVelocity(Vector3 startPoint, Vector3 targetPoint, float flightTime)
        {
            Vector3 toTarget = targetPoint - startPoint;
            Vector3 toTargetXZ = new Vector3(toTarget.x, 0, toTarget.z);

            float y = toTarget.y;
            float xz = toTargetXZ.magnitude;

            float vxz = xz / flightTime;
            float vy = y / flightTime + 0.5f * Mathf.Abs(Physics.gravity.y) * flightTime;

            Vector3 result = toTargetXZ.normalized * vxz;
            result.y = vy;

            return result;
        }

        public void Throw(Vector3 throwStartPoint)
        {
            this.isThrown = true;
            rigid.isKinematic = false;

            //Vector3 velocity = CalculateParabolicVelocity(throwStartPoint, targetPosition, 1.0f);

            Vector3 direction = Camera.main.transform.forward;
            direction.y += 1.0f; // °î¼± ºñÀ² Á¶Á¤
            direction.Normalize();

            rigid.AddForce(direction * 6.0f, ForceMode.Impulse);
            Destroy(this, lifeTime);
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
            Vector3 startPos = startPosition.position;
            Vector3 velocity = Camera.main.transform.forward * 6.0f; // ´øÁö´Â Èû * 6
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
