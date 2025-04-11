using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using UnityEngine;

namespace TST
{
    [RequireComponent(typeof(LineRenderer))]
    public class TrajectoryPredictor : MonoBehaviour
    {
        [SerializeField]
        LineRenderer trajectoryLine;
        [SerializeField, Tooltip("Collider 히트 시 Hit 마커 생성")]
        Transform hitMarker;
        [SerializeField, Range(10, 100), Tooltip("LineRenderer가 가질 수 있는 최대 좌표 갯수")]
        int maxPoints = 50;
        [SerializeField, Range(0.01f, 0.5f), Tooltip("시간 증가량 제한 범위 (궤도 계산을 위한)")]
        float increment = 0.025f;
        [SerializeField, Range(1.05f, 2f), Tooltip("The raycast overlap between points in the trajectory, this is a multiplier of the length between points. 2 = twice as long")]
        float rayOverlap = 1.1f;

        private void Start()
        {
            if (trajectoryLine == null)
                trajectoryLine = GetComponent<LineRenderer>();

            SetTrajectoryVisible(true);
        }

        // Mass : 오브젝트의 질량 ( 기본 단위는 Kg 단위 )
        // Drag : 오브젝트가 힘으로 움직일 때 공기 저항력의 값.
        // Angular Drag : 오브젝트가 토크로 회전 할 때 공기 저항력.

        public void PredictTrajectory(ProjectileProperties projectile)
        {
            Vector3 velocity = projectile.initialSpeed / projectile.mass * projectile.direction;
            Vector3 position = projectile.initialPosition;
            Vector3 nextPosition;
            float overlap;

            // 속력과 다음 위치를 예측
            velocity = CalculateNewVelocity(velocity, projectile.drag, increment);
            nextPosition = position + velocity * increment;

            for (int i = 0; i < maxPoints; i++)
            {
                // Estimate velocity and update next predicted position
                velocity = CalculateNewVelocity(velocity, projectile.drag, increment);
                nextPosition = position + velocity * increment;

                // 표면을 놓치지 않도록 광선을 약간 겹치게 한다. 판정을 조금 더 빡빡하게
                overlap = Vector3.Distance(position, nextPosition) * rayOverlap;

                //When hitting a surface we want to show the surface marker and stop updating our line
                if (Physics.Raycast(position, velocity.normalized, out RaycastHit hit, overlap, 0))
                {
                    UpdateLineRender(i, (i - 1, hit.point));
                    MoveHitMarker(hit);
                    return;
                }

                //If nothing is hit, continue rendering the arc without a visual marker
                hitMarker.gameObject.SetActive(false);
                position = nextPosition;
                UpdateLineRender(maxPoints, (i, position)); //Unneccesary to set count here, but not harmful
            }
        }

        private Vector3 CalculateNewVelocity(Vector3 velocity, float drag, float increment)
        {
            velocity += Physics.gravity * increment;
            velocity *= Mathf.Clamp01(1f - drag * increment);
            return velocity;
        }

        // 튜플 활용
        private void UpdateLineRender(int count, (int point, Vector3 pos) pointPos)
        {
            trajectoryLine.positionCount = count;
            trajectoryLine.SetPosition(pointPos.point, pointPos.pos);
        }

        private void MoveHitMarker(RaycastHit hit)
        {
            hitMarker.gameObject.SetActive(true);

            float offset = 0.025f;
            hitMarker.position = hit.point + hit.normal * offset;
            hitMarker.rotation = Quaternion.LookRotation(hit.normal, Vector3.up);
        }

        public void SetTrajectoryVisible(bool visible)
        {
            trajectoryLine.enabled = visible;
            hitMarker.gameObject.SetActive(visible);
        }
    }
}
