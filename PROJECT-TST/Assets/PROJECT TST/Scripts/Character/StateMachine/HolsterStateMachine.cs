using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class HolsterStateMachine : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var character = animator.GetComponent<CharacterBase>();
            if (character != null)
            {
                character.HolsterStart();
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var character = animator.GetComponent<CharacterBase>();
            if (character != null)
            {
                character.HolsterFinished();
            }
        }
    }
}
