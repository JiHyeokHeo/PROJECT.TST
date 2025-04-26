using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class StateMachineBase : StateMachineBehaviour
    {
        protected CharacterBase character;

        public virtual void Init(CharacterBase character)
        {
            this.character = character;
        }
    }
}
