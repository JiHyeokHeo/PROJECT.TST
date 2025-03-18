using System;
using System.Collections;
using System.Collections.Generic;
using TST;
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
            UserDataModel.Singleton.OnPlayerEquipmentChanagedEvent += RefreshEquipment;
        }

        public void OnDisable()
        {
            if (UserDataModel.Singleton)
                UserDataModel.Singleton.OnPlayerEquipmentChanagedEvent -= RefreshEquipment;
        }

        // 조건이 세가지 존재함
        // 1. 그냥 해제 할때
        // 2. 장착 할때
        // 3. 장착 중인 장비 위에 장착을 할 때 <- 귀찮은 케이스
        public void RefreshEquipment(ItemEquipmentCategory category, int beforeItemSlotID, int afterItemSlotID)
        {
            ItemData beforeItemData = null;
            ItemData afterItemData = null;

            if (beforeItemSlotID >= 0)
            {
                var beforeUserItemData = UserDataModel.Singleton.UserItemData.Items.Find(x => x.slotID == beforeItemSlotID);
                GameDataModel.Singleton.GetItemData(beforeUserItemData.itemID, out beforeItemData);
            }

            if (afterItemSlotID >= 0)
            {
                var afterUserItemData = UserDataModel.Singleton.UserItemData.Items.Find(x => x.slotID == afterItemSlotID);
                GameDataModel.Singleton.GetItemData(afterUserItemData.itemID, out afterItemData);
            }

            if (beforeItemData != null)
            {
                // 기존 장착이 해제 된 만큼 스탯을 빼준다
                EquipmentStat equipmentStat = (EquipmentStat)beforeItemData.ItemStat;
                ReduceEqipmentStatus(beforeItemData, equipmentStat);
            }

            if (afterItemData != null) 
            {
                // 새로 장착한 아이템이 있다 => 스탯을 갱신해준다
                EquipmentStat equipmentStat = (EquipmentStat)afterItemData.ItemStat;
                AddEquipmentStatus(afterItemData, equipmentStat);
            }
        }

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
