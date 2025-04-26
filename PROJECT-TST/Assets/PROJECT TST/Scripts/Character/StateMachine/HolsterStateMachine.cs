using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class HolsterStateMachine : StateMachineBase
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (character != null)
            {
                character.HolsterStart();
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (character != null)
            {
                character.HolsterFinished();
            }
        }
    }
}
