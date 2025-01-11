using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class EquipStateMachine : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var character = animator.GetComponent<CharacterBase>();
            if (character != null)
            {
                character.EquipFinished();
            }
        }
    }
}
