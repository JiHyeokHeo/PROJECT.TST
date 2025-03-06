using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace TST
{
    public class Door : MonoBehaviour, IInteractable
    {
        public string Message => "문 상호작용";

        public InteractType InteractType => InteractType.Door;

        public float openRotationMax = 60.0f;
        public float closeRotation = 0.0f;

        private bool isOpen = false;

        public float sqrInteractRange = 5f;
        private Transform doorTransform;
        private Vector3 targetRotation;

        void Awake()
        {
            doorTransform = GetComponent<Transform>();
        }

        public void Interact(GameObject go)
        {
            if (go.TryGetComponent(out CharacterBase playerComponent) == false)
                return;

            Vector3 doorPos = this.gameObject.transform.position;
            Vector3 playerPos = playerComponent.transform.position;

            float sqrDistMagnitude = Vector3.SqrMagnitude(doorPos - playerPos);
            if (sqrDistMagnitude > sqrInteractRange)
                return;

            // 열렸으면 닫혀야하고 닫혔으면 열려야한다 // 방향에 따라 달라야함 
            targetRotation = doorTransform.transform.rotation.eulerAngles;
            targetRotation.y = isOpen ? closeRotation : openRotationMax;

            Vector3 dir = playerComponent.transform.position - doorTransform.position; 
            Vector3 doorForward = doorTransform.forward;
            float dotResult = Vector3.Dot(doorForward, dir);

            // 후면에 있으면 y값만 변경
            if (dotResult < 0)
                targetRotation.y *= -1;

            isOpen = !isOpen;
            //playerComponent.SetInteractAnimation(EInteractionType.OpenDoor);
        }

        private void Update()
        {
            doorTransform.transform.rotation = Quaternion.Lerp(doorTransform.transform.rotation, Quaternion.Euler(targetRotation), Time.deltaTime * 10.0f);
        }

    }
}
