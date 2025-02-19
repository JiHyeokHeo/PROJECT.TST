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

        private void Start()
        {
            linkedCharacter = GetComponent<CharacterBase>();
        }

        public void OnEnable()
        {
            GameManager.Instance.OnChangedEquipment += RefreshEquipment;
        }

        public void OnDisable()
        {
            GameManager.Instance.OnChangedEquipment -= RefreshEquipment;
        }

        // 스탯 전환 ㄱㄱ
        public void RefreshEquipment(ItemData itemdata)
        {
            //
            Debug.Log("장비 전환 시작");
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
