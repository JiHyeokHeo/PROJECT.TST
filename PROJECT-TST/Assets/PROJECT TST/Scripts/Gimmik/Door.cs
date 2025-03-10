using TST;
using UnityEngine;

 namespace TST
{
    public class Door : MonoBehaviour, IInteractable
    {
        [SerializeField] public Transform doorHandlerTransform;

        public string Message => "문 상호작용";
        public InteractType InteractType => InteractType.Door;

        public float openRotationMax = 150.0f;
        private float closeRotation;
        private bool isOpen = false;

        public float sqrInteractRange = 10f;
        private Quaternion targetRotation;

        void Awake()
        {
            closeRotation = doorHandlerTransform.eulerAngles.y; // 올바른 y 회전값 설정
            targetRotation = doorHandlerTransform.rotation;
        }

        public void Interact(GameObject go)
        {
            if (!go.TryGetComponent(out CharacterBase playerComponent))
                return;

            Vector3 doorPos = doorHandlerTransform.position;
            Vector3 playerPos = playerComponent.transform.position;

            float sqrDistMagnitude = Vector3.SqrMagnitude(doorPos - playerPos);
            if (sqrDistMagnitude > sqrInteractRange)
                return;

            // 플레이어의 위치에 따라 문이 열리는 방향 결정
            Vector3 dir = (playerPos - doorHandlerTransform.position).normalized;
            Vector3 doorForward = doorHandlerTransform.forward;
            float dotResult = Vector3.Dot(doorForward, dir);

            float openAngle = isOpen ? closeRotation : closeRotation + openRotationMax;

            // 문 반대쪽에서 상호작용하면 반대 방향으로 열림
            if (dotResult < 0 && !isOpen)
                openAngle = closeRotation - openRotationMax;

            targetRotation = Quaternion.Euler(0, openAngle, 0);
            isOpen = !isOpen;
        }

        private void Update()
        {
            doorHandlerTransform.rotation = Quaternion.Lerp(doorHandlerTransform.rotation, targetRotation, Time.deltaTime * 5.0f);
        }
    }
}
