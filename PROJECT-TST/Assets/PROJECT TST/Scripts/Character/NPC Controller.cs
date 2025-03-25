using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class NPCController : MonoBehaviour, IInteractable
    {
        private Animator animator;

        private bool isAnimatorReverseStart = false;

        [field: SerializeField] public string NpcName { get; set; }
        public string Message => "NPC Interact";
        public InteractType InteractType => InteractType.NPC;

        void Start()
        {
             animator = GetComponent<Animator>();
            
            //Animator speed
        }

        public void Ani_Reverse()
        {
            isAnimatorReverseStart = true;
            animator.SetFloat("Animator speed", -1f);
        }

        // lookcount가 1이면 한번 머리를 꺽고 다시 돌아가는 걸 체크하는 이벤트를 추가함
        // 전환 트리거를 발생시키자.
        public void Ani_LookTransitionCheck()
        {
            if (isAnimatorReverseStart)
            {
                animator.SetFloat("Animator speed", 1f);
                animator.SetTrigger("Animation Transition Trigger");
                isAnimatorReverseStart = false;
            }
        }

        public void Interact(GameObject go)
        {
            // 관련된 상점 UI 팝업
        }
    }
}
