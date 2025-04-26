using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class ThrowStateMachine : StateMachineBase
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animator != null)
            {
                if (character != null) 
                {
                    animator.SetBool("IsThrown", false);
                    if (character.currentThrowObject == null)
                        character.IsThrowMode = true;
                }
            }
        }
    }
}
