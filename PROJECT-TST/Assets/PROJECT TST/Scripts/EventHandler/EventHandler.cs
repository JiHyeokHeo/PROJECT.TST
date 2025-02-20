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

        public void RefreshEquipment(ItemData itemdata, bool isEquip)
        {
            // 여기서 장비 관련 스탯 전환 // 기존에 끼던 스탯을 내리고 
            Debug.Log("장비 전환 시작");

            if (isEquip)
            {

            }
            else
            {

            }
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
