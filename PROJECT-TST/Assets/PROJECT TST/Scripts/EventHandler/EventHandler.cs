using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class EventHandler : MonoBehaviour
    {
        private CharacterBase linkedCharacter;
        public event Action OnDamagedAction;
        public event Action OnDeadAction;

        public CharacterBase LinkCharaceter(CharacterBase linkedCharacter)
        {
            this.linkedCharacter = linkedCharacter;

            return linkedCharacter;
        }

        public void OnDamaged(float damage, GameObject attacker)
        {
            Debug.Log($"{attacker.name}로부터 {damage}데미지 를 받는 중 ");
            linkedCharacter.CurrentHp -= damage;

            OnDamagedAction?.Invoke();

            if (linkedCharacter.CurrentHp <= 0 )
            {
                linkedCharacter.CurrentHp = 0;
                Debug.Log($" {linkedCharacter.name} Died.");
                OnDeadAction?.Invoke();
            }
        }
    }
}
