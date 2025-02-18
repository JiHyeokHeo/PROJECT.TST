using Gpm.Ui;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class InventoryUI : UIBase
    {
        public CharacterBase character;
        [SerializeField] private Transform itemSlotRoot;
        [SerializeField] private InventoryUI_ItemSlot itemSlotPrefab;
        [SerializeField] private InfiniteScroll infiniteScroll;

        private List<InventoryUI_ItemData> inventoryDatas = new List<InventoryUI_ItemData>();

        private void Awake()
        {
            UserDataModel.Singleton.OnUserItemChangedEvent += OnChangedUserItemData;
            itemSlotPrefab.gameObject.SetActive(false);
        }

        public void FixedUpdate()
        {

        }

        private void OnEnable()
        {
            // TODO : UserDataModel의 UserItemData에 있는 아이템 데이터를 읽고, 인벤토리에 표기해준다.
            GameManager.Instance.OnUsedItem += RefreshInventory;

            RefreshInventory(null, 0);
        }

        private void OnDisable()
        {
            GameManager.Instance.OnUsedItem -= RefreshInventory;
        }

        private void RefreshInventory(ItemData itemData, int count)
        {
            infiniteScroll.ClearData();

            for (int i = 0; i < UserDataModel.Singleton.UserItemData.Items.Count; i++)
            {
                InventoryUI_ItemData inventoryItemData = new InventoryUI_ItemData();
                if (GameDataModel.Singleton.GetItemData(UserDataModel.Singleton.UserItemData.Items[i].itemID, out var itemGameData))
                {
                    inventoryItemData.itemSlotId = UserDataModel.Singleton.UserItemData.Items[i].slotID;
                    inventoryItemData.itemData = itemGameData;
                    inventoryItemData.itemCount = UserDataModel.Singleton.UserItemData.Items[i].itemCount;
                }

                infiniteScroll.InsertData(inventoryItemData);
            }
        }
  
        private void OnChangedUserItemData(UserItemDTO.UserItemData data)
        {
            // TODO : 인벤토리에 표기하는 아이템들은 Dictionary<int, InventoryUI_ItemSlot> createdItemSlots 에 저장하고 관리한다.
            // TODO : Dictionary<int, InventoryUI_ItemSlot> 의 int Key 값은 SlotID 와 동일하다.
            if (data == null)
                return;

            // 만약에 데이터를 가지고 있다면 데이터 정보 변경후 인피니티 스크롤 업데이트 후 리턴
            int existIndex = inventoryDatas.FindLastIndex(x => x.itemSlotId.Equals(data.slotID));
            if (existIndex >= 0)
            {
                var exisitedInventoryData = inventoryDatas[existIndex];
                ChangeItemData(exisitedInventoryData, data);
                infiniteScroll.UpdateData(exisitedInventoryData);
                return;
            }

            // 데이터를 가지고 있지 않다면 데이터 Add
            InventoryUI_ItemData itemData = new InventoryUI_ItemData();
            ChangeItemData(itemData, data);

            inventoryDatas.Add(itemData);
            infiniteScroll.InsertData(itemData);
        }

        private void ChangeItemData(InventoryUI_ItemData itemData, UserItemDTO.UserItemData receiveData)
        {
            if (GameDataModel.Singleton.GetItemData(receiveData.itemID, out ItemData itemGameData))
            {
                itemData.itemSlotId = receiveData.slotID;
                itemData.itemData = itemGameData;
                itemData.itemCount = receiveData.itemCount;
            }
        }

        public void OnClickCloseButton()
        {
            InputSystem.Singleton.ChangeCursorVisibility(false);
            UIManager.Hide<InventoryMenuUI>(UIList.InventoryMenuUI);
            UIManager.Hide<InventoryUI>(UIList.InventoryUI);
        }

        public void OnNotifyOnClickItemSlot(InventoryUI_ItemData inventoryItemData)
        {
            GameManager.Instance.UseItem(inventoryItemData.itemData);
        }

        public void OnNotifyOnClickItemSlot(string itemId, int useCount)
        {
            if (GameDataModel.Singleton.GetItemData(itemId, out ItemData resultData))
            {
                GameManager.Instance.UseItem(resultData, useCount);
            }
        }

        public void SetLinkedCharacter(CharacterBase character)
        {
            this.character = character;
        }
    }
}
