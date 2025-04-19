using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public enum NpcType
    {
        Store,
        Start,
        Ending,
    }

    public class NPCController : MonoBehaviour, IInteractable
    {
        private Animator animator;

        private bool isAnimatorReverseStart = false;

        [field: SerializeField] public string NpcName { get; set; }
        public string Message => "NPC Interact";
        public InteractType InteractType => InteractType.NPC;
        [field: SerializeField] public NpcType npcType { get; set; }

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
            switch (npcType)
            {
                case NpcType.Store:
                    UIManager.Show<NpcShopUI>(UIList.NpcShopUI);
                    break;
                case NpcType.Start:
                    UIManager.Show<NpcTextScript>(UIList.NpcScriptUI).RegisterDialogue(this);
                    break;
                case NpcType.Ending:
                    UIManager.Show<NpcTextScript>(UIList.NpcScriptUI).RegisterDialogue(this); 
                    break;
            }
        }

        public void OnTriggerExit(Collider other)
        {
            // 관련된 상점 UI 팝업
            switch (npcType)
            {
                case NpcType.Store:
                    UIManager.Hide<NpcShopUI>(UIList.NpcShopUI);
                    break;
                case NpcType.Start:
                    UIManager.Hide<NpcTextScript>(UIList.NpcScriptUI); 
                    break;
                case NpcType.Ending:
                    UIManager.Hide<NpcTextScript>(UIList.NpcScriptUI); 
                    break;
            }
            
        }
    }
}
