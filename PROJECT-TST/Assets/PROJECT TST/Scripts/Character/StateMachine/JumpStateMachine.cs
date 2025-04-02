using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class JumpStateMachine : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // 상태에 진입할 때 호출됩니다.
            var character = animator.GetComponent<CharacterBase>();
            if (character != null)
            {
                character.JumpStart();
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var character = animator.GetComponent<CharacterBase>();
            if (character != null)
            {
                character.JumpFinished();
            }
        }
    }
}
