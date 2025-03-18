using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class NPCController : MonoBehaviour
    {
        private Animator animator;

        private int lookCount;
        void Start()
        {
             animator = GetComponent<Animator>();
            lookCount = 0;
            //Animator speed
        }

        public void Ani_Reverse()
        {
            lookCount++;
            animator.SetFloat("Animator speed", -1f);
        }

        // lookcount가 1이면 한번 머리를 꺽고 다시 돌아가는 걸 체크하는 이벤트를 추가함
        // 전환 트리거를 발생시키자.
        public void Ani_LookTransitionCheck()
        {
            if (lookCount >= 1)
            {
                animator.SetFloat("Animator speed", 1f);
                lookCount = 0;
            }
        }
    }
}
