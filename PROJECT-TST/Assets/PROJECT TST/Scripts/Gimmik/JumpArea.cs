using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace TST
{
    public class JumpArea : MonoBehaviour, IInteractable
    {
        public string Message => "Jump Start";

        public InteractType InteractType => InteractType.None;

        [SerializeField] public OffMeshLink NavMeshLink;

        public void Interact(GameObject go)
        {
            if (go.TryGetComponent<NavMeshAgent>(out var agent))
            {
                if (go.TryGetComponent<UnityEngine.CharacterController>(out var controller))
                    controller.enabled = false;

                go.GetComponent<CharacterBase>().Jump();
                go.GetComponent<MonoBehaviour>().StartCoroutine(ReactivateController(agent, controller));
                agent.SetDestination(NavMeshLink.endTransform.position);
                StartCoroutine(HandleParabolicJump(agent));
            }
        }

        private IEnumerator ReactivateController(NavMeshAgent agent, UnityEngine.CharacterController controller)
        {
            // OffMeshLink 통과할 때까지 대기
            while (!agent.isOnOffMeshLink)
                yield return null;

            // OffMeshLink 완료 대기
            while (agent.isOnOffMeshLink)
                yield return null;

            // 잠시 후 다시 켜기 (충돌 안정화용 딜레이)
            yield return new WaitForSeconds(0.1f);

            if (controller != null)
            {
                controller.enabled = true;
            }
        }

        private IEnumerator HandleParabolicJump(NavMeshAgent agent, float duration = 1.0f, float jumpHeight = 1.5f)
        {
            Vector3 startPos = agent.transform.position;
            Vector3 endPos = NavMeshLink.endTransform.position+ Vector3.up * 0.1f; // 보정값 살짝 추가

            float time = 0f;

            while (time < duration)
            {
                float t = time / duration;
                float height = 4f * jumpHeight * t * (1 - t); // 부드러운 포물선

                agent.transform.position = Vector3.Lerp(startPos, endPos, t) + Vector3.up * height;

                time += Time.deltaTime;
                yield return null;
            }

            agent.CompleteOffMeshLink();
            agent.updatePosition = true;
            agent.updateRotation = true;
        }
    }
}
