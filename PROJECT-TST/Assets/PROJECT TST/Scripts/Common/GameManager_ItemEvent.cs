using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;

namespace TST
{
    public partial class GameManager : MonoBehaviour
    {
        public event System.Action<ItemData, int> OnUsedItem;

        public void UseItem(int slotId, ItemData itemData, int count = 1)
        {
            switch (itemData.ItemCategory)
            {
                case ItemCategory.Equipment:
                    EquipmentItem(slotId, itemData);
                    break;
                case ItemCategory.Material:
                    break;
                case ItemCategory.Consumable:
                     UseConsumable(slotId, itemData, count);
                    UserDataModel.Singleton.UseInventoryItem(itemData, count);
                    break;
            }

            // 마지막에 사용한 아이템을 UserDataModel에서 삭제하도록 처리를 불러주자
            OnUsedItem?.Invoke(itemData, count);
        }

        public void UnEquipmentItem(ItemEquipmentCategory category, int slotId)
        {
            UserDataModel.Singleton.UnEquipItem(category, slotId);
        }

        private void EquipmentItem(int slotId, ItemData itemData)
        {
            // 실제 데이터 변경은 이쪽에서
            var category = (ItemEquipmentCategory)itemData.ItemSubCategory;
            UserDataModel.Singleton.EquipItem(category, slotId);
        }

        private void UseConsumable(int slotId, ItemData itemData, int count = 1)
        {
            switch (itemData.ItemSubCategory)
            {
                case (int)ItemConsumableCategory.HealingKit:
                    {
                        ItemStatBase stat = itemData.ItemStat;
                        if (stat == null)
                        {
                            Assert.IsNull(stat, $"{stat} is Not Valid");
                            return;
                        }

                        Debug.Log("HealingKit Used");
                        if (stat is ConsumableStat consumableStat)
                        {
                            CharacterController.Instance.linkedCharacter.CurrentHp += consumableStat.buffValue;
                        }
                        break;
                    }
                case (int)ItemConsumableCategory.Ammo:
                    {
                        ItemStatBase stat = itemData.ItemStatSubAdded;

                        if (stat == null)
                        {
                            Assert.IsNull(stat, $"{stat} is Not Valid");
                            return;
                        }

                        if (stat is AmmoStat consumableStat)
                        {
                            CharacterBase user = CharacterController.Instance.linkedCharacter;
                            int existRifleIndex = user.rifleAmmos.FindLastIndex(x => x.data.AmmoType.Equals(consumableStat.AmmoType));
                            int existPistolIndex = user.pistolAmmos.FindLastIndex(x => x.data.AmmoType.Equals(consumableStat.AmmoType));
                            if (existRifleIndex >= 0)
                            {
                                user.rifleAmmos[existRifleIndex].CurrentAmmo += consumableStat.bulletAmount;
                                user.primaryWeapon.AddMaxAmountBullet(consumableStat.bulletAmount);
                            }

                            if (existPistolIndex >= 0)
                            {
                                user.pistolAmmos[existPistolIndex].CurrentAmmo += consumableStat.bulletAmount;
                                user.subWeapon.AddMaxAmountBullet(consumableStat.bulletAmount);
                            }
                        }
                        break;
                    }
            }
        }

     
    }
}
