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
        public event System.Action<ItemData> OnEquipItem;

        public void UseItem(ItemData itemData, int count = 1)
        {
            switch (itemData.ItemCategory)
            {
                case ItemCategory.Equipment:
                    EquipmentItem(itemData, count);
                    OnEquipItem?.Invoke(itemData);
                    break;
                case ItemCategory.Material:
                    break;
                case ItemCategory.Consumable:
                    {
                        UseConsumable(itemData, count);
                    }
                    break;
            }

            // 마지막에 사용한 아이템을 UserDataModel에서 삭제하도록 처리를 불러주자
            UserDataModel.Singleton.UseInventoryItem(itemData, count);
            OnUsedItem?.Invoke(itemData, count);
        }

        private void EquipmentItem(ItemData itemData, int count = 1)
        {
            switch (itemData.ItemSubCategory) 
            {
                case (int)ItemEquipmentCategory.Helmet:
                    Debug.Log("Equip Helmet");
                    break;
                case (int)ItemEquipmentCategory.Weapon:
                    Debug.Log("Equip Weapon");
                    break;
                case (int)ItemEquipmentCategory.Gloves:
                    Debug.Log("Equip Gloves");
                    break;
                case (int)ItemEquipmentCategory.Armor:
                    Debug.Log("Equip Armor");
                    break;
                case (int)ItemEquipmentCategory.Shoes:
                    Debug.Log("Equip Shoes");
                    break;

            }
        }

        private void UseConsumable(ItemData itemData, int count = 1)
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
