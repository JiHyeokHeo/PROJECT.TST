using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class EquipStateMachine : StateMachineBase
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (character != null)
            {
                character.EquipFinished();
            }
        }
    }
}
