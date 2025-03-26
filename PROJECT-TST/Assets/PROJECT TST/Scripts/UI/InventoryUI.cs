using Gpm.Ui;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace TST
{
    public class InventoryUI : UIBase
    {
        public override bool IsVisibleCursor => true;

        public CharacterBase character;
        [SerializeField] private Transform itemSlotRoot;
        [SerializeField] private InventoryUI_ItemSlot itemSlotPrefab;
        [SerializeField] private InfiniteScroll infiniteScroll;
        
        private void Awake()
        {
            itemSlotPrefab.gameObject.SetActive(false);
        }

        public void FixedUpdate()
        {

        }

        private void OnEnable()
        {
            // TODO : UserDataModel의 UserItemData에 있는 아이템 데이터를 읽고, 인벤토리에 표기해준다.
            if (GameManager.Instance)
                GameManager.Instance.OnUsedItem += (_ , _) => RefreshInventory();

            if (UserDataModel.Singleton)
            {
                UserDataModel.Singleton.OnUserItemChangedEvent += OnChangedUserItemData;
                UserDataModel.Singleton.OnPlayerEquipmentChanagedEvent += (_, _, _) => RefreshInventory();
            }

            RefreshInventory();
        }

        private void OnDisable()
        {
            if (GameManager.Instance)
                GameManager.Instance.OnUsedItem -= (_, _) => RefreshInventory();

            if (UserDataModel.Singleton)
            {
                UserDataModel.Singleton.OnUserItemChangedEvent -= OnChangedUserItemData;
                UserDataModel.Singleton.OnPlayerEquipmentChanagedEvent -= (_, _, _) => RefreshInventory();
            }
        }

        private void RefreshInventory()
        {
            infiniteScroll.ClearData();

            for (int i = 0; i < UserDataModel.Singleton.UserItemData.Items.Count; i++)
            {
                // 데이터 가져오는거 1차 체크
                if (GameDataModel.Singleton.GetItemData(UserDataModel.Singleton.UserItemData.Items[i].itemID , out ItemData itemGameData))
                {
                    if (itemGameData.ItemID.Equals("Money"))
                        continue;

                    // 장비 아이템인 경우
                    if (itemGameData.ItemCategory == ItemCategory.Equipment)
                    {
                        // 장비 아이템이 혹시나 장착 중인 녀석이라면?
                        if (UserDataModel.Singleton.PlayerEquipmentData.equipmentItems.ContainsValue(UserDataModel.Singleton.UserItemData.Items[i].slotID))
                        {
                            continue;
                        }

                    }
                }

                InventoryUI_ItemData inventoryItemData = new InventoryUI_ItemData();
                inventoryItemData.itemSlotId = UserDataModel.Singleton.UserItemData.Items[i].slotID;
                inventoryItemData.itemData = itemGameData;
                inventoryItemData.itemCount = UserDataModel.Singleton.UserItemData.Items[i].itemCount;

                infiniteScroll.InsertData(inventoryItemData);
            }
        }
        
        // 돈이라는 아이템을 먹은 것만 예외처리로 Inventory Gold Hud에 영향을 미치도록 변경? // 아니면 UserDataModel에 따로 함수를 파고 ItemBase에서 Interact 를 if문으로 미리 필터링?
        private void OnChangedUserItemData(UserItemDTO.UserItemData data)
        {
            // TODO : 인벤토리에 표기하는 아이템들은 Dictionary<int, InventoryUI_ItemSlot> createdItemSlots 에 저장하고 관리한다.
            // TODO : Dictionary<int, InventoryUI_ItemSlot> 의 int Key 값은 SlotID 와 동일하다.
            if (data == null)
                return;

            // UIHud에 돈 관련 Text Image 추가
            if (data.itemID.Equals("Money"))
            {

                return;
            }

            InventoryUI_ItemData itemData = new InventoryUI_ItemData();
            if (GameDataModel.Singleton.GetItemData(data.itemID, out var itemGameData))
            {
                itemData.itemSlotId = data.slotID;
                itemData.itemData = itemGameData;
                itemData.itemCount = data.itemCount;
            }
            infiniteScroll.InsertData(itemData);

            RefreshInventory();
        }
 
        public void OnClickCloseButton()
        {
            InputSystem.Singleton.ChangeCursorVisibility(false);
            UIManager.Hide<InventoryMenuUI>(UIList.InventoryMenuUI);
            UIManager.Hide<InventoryUI>(UIList.InventoryUI);
        }

        public void OnNotifyOnClickItemSlot(int slotId, int useCount = 1)
        {
            InventoryUI_ItemData targetInventoryItemData = null;
            var inventoryDataList = infiniteScroll.GetDataList();
            for (int i =0; i < inventoryDataList.Count; i++)
            {
                var castingData = inventoryDataList[i] as InventoryUI_ItemData;
                if (castingData.itemSlotId == slotId)
                {
                    targetInventoryItemData = castingData;
                    break;
                }
            }

            if (targetInventoryItemData != null)
            {
                GameManager.Instance.UseItem(targetInventoryItemData.itemSlotId, targetInventoryItemData.itemData, useCount);
            }
        }

        public void SetLinkedCharacter(CharacterBase character)
        {
            this.character = character;
        }
    }
}
