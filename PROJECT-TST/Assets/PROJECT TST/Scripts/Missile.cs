using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class Missile : MonoBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private Rigidbody rb;
        [SerializeField] private Target target;
        [SerializeField] private GameObject explosionPrefab;

        [Header("MOVEMENT")]
        [SerializeField] private float speed = 5f;
        [SerializeField] private float rotateSpeed = 180f;

        // 예측
        [Header("PREDICTION")]
        [SerializeField] private float maxDistancePredict = 100;
        [SerializeField] private float minDistancePredict = 0.1f;
        [SerializeField] private float maxTimePrediction = 5;
        private Vector3 standardPrediction, deviatedPrediction;

        // 편차
        [Header("DEVIATION")]
        [SerializeField] private float _deviationAmount = 5;
        [SerializeField] private float _deviationSpeed = 2;

        private float initLaunctTime = 0f;
        private float initMaxLaunchTime = 1f; // 첫 1초간은 그냥 내가 시작점으로 붕 날라가도록

        public Vector3 initRotation = Vector3.zero;

        private void Start()
        {
            // Euler 각도를 Quaternion으로 변환하여 회전에 적용
            //new Vector3(-40.0f, 66.0f, 0.0f)
            Quaternion initialRotation = Quaternion.Euler(initRotation);
            transform.rotation = initialRotation;
            GameObject obj = EffectManager.Singleton.SpawnEffect(EffectType.SmokeTrail);
            obj.transform.parent = transform;
            obj.transform.localPosition = Vector3.zero;
        }

        private void FixedUpdate()
        {
            if (gameObject.activeSelf == false)
                return;

            // 진행만 시키도록 하고
            rb.velocity = transform.forward * speed;

            initLaunctTime += Time.fixedDeltaTime;

            if (target == null /*|| initLaunctTime >= initMaxLaunchTime*/)
                return;
            // 정규화 a~b 0~1 value
            // 가까울 수록 편차가 줄고, 멀수록 편차가 커짐
            var leadTimePercentage = Mathf.InverseLerp(minDistancePredict, maxDistancePredict, Vector3.Distance(transform.position, target.transform.position));

            // 1차 예측
            PredictMovement(leadTimePercentage);

            // 편차 추가
            AddDeviation(leadTimePercentage);

            // 로테이션
            RotateRocket();
        }

        public void SetInitRotation(Vector3 rotation)
        {
            initRotation = rotation;
        }

        public void SetTarget(Target target)
        {
            this.target = target;
        }

        private void PredictMovement(float leadTimePercentage)
        {
            var predictionTime = Mathf.Lerp(0, maxTimePrediction, leadTimePercentage);

            // 예측 위치 계산의 수학적 원리 // 주로 유도 미사일, AI추적, 스포츠게임(플레이어와 공 이동 예상 인터셉트)
            // 예측 위치 = 현재위치 + (속도 x 시간)

            if (target.Rb != null)
            {
                standardPrediction = target.Rb.position + target.Rb.velocity * predictionTime;
            }
            
            if (target.Controller != null)
            {
                standardPrediction = target.transform.position + target.Controller.velocity * predictionTime;
            }
        }

        // 예측된 위치에 편차를 추가하는 기능
        private void AddDeviation(float leadTimePercentage)
        {
            // 목표의 예측 위치를 x축 방향으로 흔들리게 하는 역할을 합니다. // Cos보다 PerlinNoise를 활용하면 더욱 예측하기 어려운 움직임이 가능하다.
            var deviation = new Vector3(Mathf.Cos(Time.time * _deviationSpeed), 0, 0);

            // world 좌표의 방향 * 스칼라값 * 예측 시간에 따라 편차 크기 변경 -> 예측 시간이 짧을 수록 편차가 적다
            var predictionOffset = transform.TransformDirection(deviation) * _deviationAmount * leadTimePercentage;

            deviatedPrediction = standardPrediction + predictionOffset;
        }

        private void RotateRocket()
        {
            // 도착 방향벡터
            var heading = deviatedPrediction - transform.position;
             
            var rotation = Quaternion.LookRotation(heading);
        
            rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotation, rotateSpeed * Time.deltaTime));
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (explosionPrefab) Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            if (collision.transform.TryGetComponent<Target>(out var ex))
                ex.ApplyDamage(1, collision.gameObject);

            Destroy(gameObject);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, standardPrediction);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(standardPrediction, deviatedPrediction);
        }
    }
}
