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

        // 아이템 번호로 하면 좋더 좋았을듯
        public void BuyItem(ItemData itemData)
        {
            // 골드 유무를 확인해야함 
            

            UserDataModel.Singleton.AddItemToInventory(itemData);
        }

        public bool CraftItem(string craft_id)
        {
            return CraftingItems(craft_id);
        }

        public void GenerateItem(Vector3 position)
        {
            // 미만         이하
            // int 파라미터 float 파라미터 Range Inclusive Exclusive 
            int randItemID = UnityEngine.Random.Range((int)ItemList.ITEMLIST_START + 1, (int)ItemList.ITEMLIST_END);

            var itemEnum = (ItemList)randItemID;
            string itemName = itemEnum.ToString();

            if (AssetManager.Singleton.GetItemVisualPrefab(itemName, out GameObject result))
            {
                Instantiate(result, position, Quaternion.identity);
            }
        }

        public void UseItem(int slotId, ItemData itemData, int count = 1)
        {
            switch (itemData.ItemCategory)
            {
                case ItemCategory.Equipment:
                    EquipmentItem(slotId, itemData);
                    break;
                case ItemCategory.Material:
                    UserDataModel.Singleton.UseInventoryItem(itemData, count);
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
        
        private bool CraftingItems(string crafting_Id)
        {
            if (!GameDataModel.Singleton.GetCraftingData(crafting_Id, out CraftDataSO craftingData))
                return false;

            // 먼저 내가 소유하고 있는 아이템의 갯수가 충분 한지 확인
            if (craftingData.RequireItems.Count > 0)
            {
                for (int i = 0; i < craftingData.RequireItems.Count; i++)
                {
                    CraftingDataBase requireItemData = craftingData.RequireItems[i];

                    GameDataModel.Singleton.GetItemData(requireItemData.ItemID, out ItemData usingItemData);
                    var data = UserDataModel.Singleton.UserItemData.GetUserItemData(requireItemData.ItemID);

                    if (data == null)
                        return false;

                    if (data.itemCount < requireItemData.RequireAmount)
                        return false;

                    UseItem(-1, usingItemData, requireItemData.RequireAmount);
                }
            }

            if (GameDataModel.Singleton.GetItemData(craftingData.ResultItemID, out ItemData createdItemData))
            {
                bool isCraftingSuccess = false;

                float rand = UnityEngine.Random.Range(0.001f, 1f);
                //float successRate = createdItemData.

                //for (int i = 0; i < craftingData.ResultAmount; i++)
                //{
                    UserDataModel.Singleton.AddItemToInventory(createdItemData, craftingData.ResultAmount);
                //}
            }

            return true;
        }
    }
}
