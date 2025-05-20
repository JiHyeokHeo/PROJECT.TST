using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class LootLoopStateMachine : StateMachineBase
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (character != null)
            {
                float magnitude = animator.GetFloat("Magnitude");

                if (magnitude > 0.001f)
                {
                    character.LootLoopStopped();
                }
            }
        }
    }
}
