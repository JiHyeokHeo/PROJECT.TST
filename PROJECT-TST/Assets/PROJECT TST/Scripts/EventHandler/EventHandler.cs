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

        private List<ItemData> equipItemDatas = new List<ItemData>();
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

        // 조건이 세가지 존재함
        // 1. 그냥 해제 할때
        // 2. 장착 할때
        // 3. 장착 중인 장비 위에 장착을 할 때 <- 귀찮은 케이스

        public void RefreshEquipment(ItemData itemdata, bool isTryToEquip)
        {
            // 여기서 장비 관련 스탯 전환 // 기존에 끼던 스탯을 내리고 
            Debug.Log("장비 전환 시작");

            EquipmentStat equipmentStat = (EquipmentStat)itemdata.ItemStat;
            if (equipmentStat == null)
                return;

            if (isTryToEquip)
            {
                AddEquipmentStatus(itemdata, equipmentStat);
            }
            else
            {
                ReduceEqipmentStatus(itemdata, equipmentStat);
            }
        }

        //// 이미 장착 중인지 체크
        //private void CheckAlreadyWearing(ItemData itemdata)
        //{
        //    int exisitedIndex = equipItemDatas.FindLastIndex(x => x.ItemSubCategory.Equals(itemdata.ItemSubCategory));
        //    if (exisitedIndex >= 0)
        //    {
        //        EquipmentStat equippedItemStatus = (EquipmentStat)equipItemDatas[exisitedIndex].ItemStat;
        //        ReduceEqipmentStatus(equipItemDatas[exisitedIndex], equippedItemStatus);
        //        equipItemDatas[exisitedIndex] = itemdata; // 데이터 덮기
        //    }
        //    else
        //    {
        //        equipItemDatas.Add(itemdata);
        //    }
        //}

        private void AddEquipmentStatus(ItemData itemdata, EquipmentStat equipmentStat)
        {
            switch (itemdata.ItemSubCategory)
            {
                case (int)ItemEquipmentCategory.Helmet:

                    break;
                case (int)ItemEquipmentCategory.Gloves:

                    break;
                case (int)ItemEquipmentCategory.Weapon:

                    break;
                case (int)ItemEquipmentCategory.Armor:

                    break;
                case (int)ItemEquipmentCategory.Shoes:
                    linkedCharacter.CurrentSpeed += equipmentStat.speedBuff;
                    break;
            }
        }

        private void ReduceEqipmentStatus(ItemData itemdata, EquipmentStat equipmentStat)
        {
            switch (itemdata.ItemSubCategory)
            {
                case (int)ItemEquipmentCategory.Helmet:

                    break;
                case (int)ItemEquipmentCategory.Gloves:

                    break;
                case (int)ItemEquipmentCategory.Weapon:

                    break;
                case (int)ItemEquipmentCategory.Armor:

                    break;
                case (int)ItemEquipmentCategory.Shoes:
                    linkedCharacter.CurrentSpeed -= equipmentStat.speedBuff;
                    break;
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
