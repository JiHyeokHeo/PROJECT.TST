using System;
using System.Collections;
using System.Collections.Generic;
using TST;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
 
namespace TST
{
    public class EventHandler : MonoBehaviour
    {
        private CharacterBase linkedCharacter;
        public event Action<GameObject> OnDamagedAction;
        public event Func<float, GameObject, float> OnDamageCalculate;
        public event Action OnDeadEvent;
        public event Action<float> OnPulseAction;

        private List<ItemData> equipItemDatas = new List<ItemData>();
        private void Start()
        {
            linkedCharacter = GetComponent<CharacterBase>();
        }

        public void OnEnable()
        {
            UserDataModel.Singleton.OnPlayerEquipmentChanagedEvent += RefreshEquipment;

            if (TryGetComponent(out AICharacterController component) == false)
                OnDeadEvent += ShowGameOverUI;
        }

        public void OnDisable()
        {
            if (UserDataModel.Singleton)
                UserDataModel.Singleton.OnPlayerEquipmentChanagedEvent -= RefreshEquipment;

            if (TryGetComponent(out AICharacterController component) == false)
                OnDeadEvent += ShowGameOverUI;
        }

        private void ShowGameOverUI()
        {
            UIManager.Show<GameOverUI>(UIList.GameOverUI);
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
                    linkedCharacter.CurrentDefence += equipmentStat.defenseBuff;
                    break;
                case (int)ItemEquipmentCategory.Gloves:
                    linkedCharacter.CurrentDamage += equipmentStat.attackBuff;
                    linkedCharacter.CurrentDefence += equipmentStat.defenseBuff;
                    break;
                case (int)ItemEquipmentCategory.Weapon:
                    linkedCharacter.CurrentDamage += equipmentStat.attackBuff;
                    break;
                case (int)ItemEquipmentCategory.Armor:
                    linkedCharacter.CurrentDefence += equipmentStat.defenseBuff;
                    break;
                case (int)ItemEquipmentCategory.Shoes:
                    linkedCharacter.CurrentDefence += equipmentStat.defenseBuff;
                    linkedCharacter.CurrentSpeed += equipmentStat.speedBuff;
                    break;
            }
        }

        private void ReduceEqipmentStatus(ItemData itemdata, EquipmentStat equipmentStat)
        {
            switch (itemdata.ItemSubCategory)
            {
                case (int)ItemEquipmentCategory.Helmet:
                    linkedCharacter.CurrentDefence -= equipmentStat.defenseBuff;
                    break;
                case (int)ItemEquipmentCategory.Gloves:
                    linkedCharacter.CurrentDamage -= equipmentStat.attackBuff;
                    linkedCharacter.CurrentDefence -= equipmentStat.defenseBuff;
                    break;
                case (int)ItemEquipmentCategory.Weapon:
                    linkedCharacter.CurrentDamage -= equipmentStat.attackBuff;
                    break;
                case (int)ItemEquipmentCategory.Armor:
                    linkedCharacter.CurrentDefence -= equipmentStat.defenseBuff;
                    break;
                case (int)ItemEquipmentCategory.Shoes:
                    linkedCharacter.CurrentDefence -= equipmentStat.defenseBuff;
                    linkedCharacter.CurrentSpeed -= equipmentStat.speedBuff;
                    break;
            }
        }

        public void OnDamaged(float damage, GameObject attacker)
        {

            OnDamagedAction?.Invoke(attacker);

            float? totalDamage = OnDamageCalculate?.Invoke(damage, attacker);

            if (totalDamage.HasValue)
                linkedCharacter.CurrentHp -= (float)totalDamage;
            else
                linkedCharacter.CurrentHp -= damage;

            if (linkedCharacter.CurrentHp <= 0 )
            {
                linkedCharacter.CurrentHp = 0;
                Debug.Log($" {linkedCharacter.name} Died.");

                OnDeadEvent?.Invoke();
            }

            if (linkedCharacter.CurrentHp <= 30)
            {
                OnPulseAction?.Invoke(8f);
            }
            else
            {
                OnPulseAction?.Invoke(2f);
            }

        }

        // 추후에 더 추가 해야할듯
        public void ResetCharacter()
        {
            linkedCharacter.CurrentHp = 100;
        }
    }
}
