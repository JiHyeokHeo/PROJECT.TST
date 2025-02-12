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

        private List<InventoryUI_ItemSlot> createdItemSlots = new List<InventoryUI_ItemSlot>();
        private List<InventoryUI_ItemSlot> discardItemSlots = new List<InventoryUI_ItemSlot>();


        private void Awake()
        {
            UserDataModel.Singleton.OnUserItemChangedEvent += OnChangedUserItemData;
            UserDataModel.Singleton.OnCreatedItemIndexMoveEvent += OnIndexMoveEvent;
            itemSlotPrefab.gameObject.SetActive(false);

        }

        public void FixedUpdate()
        {
            if (discardItemSlots.Count > 0) 
            {
                for(int i = 0; i < discardItemSlots.Count; i++) 
                    Destroy(discardItemSlots[i], 3.0f);

                discardItemSlots.Clear();
            }
        }

        private void OnEnable()
        {
            // TODO : UserDataModel의 UserItemData에 있는 아이템 데이터를 읽고, 인벤토리에 표기해준다.
        }

        private void OnDisable()
        {
            
        }

        private void OnIndexMoveEvent(UserItemDTO.UserItemData data)
        {
            if (data == null)
                return;

            createdItemSlots[data.slotID].SetItem(data.itemID, data.slotID, data.itemCount);
        }

        private void OnChangedUserItemData(UserItemDTO.UserItemData data)
        {
            // TODO : 인벤토리에 표기하는 아이템들은 Dictionary<int, InventoryUI_ItemSlot> createdItemSlots 에 저장하고 관리한다.
            // TODO : Dictionary<int, InventoryUI_ItemSlot> 의 int Key 값은 SlotID 와 동일하다.
            if (data == null)
                return;

            if (createdItemSlots.Exists(x=>x.ItemSlotID == data.slotID))
            {
                int index = createdItemSlots.FindIndex(x=>x.ItemSlotID == data.slotID);
                if (index>=0)
                {
                    if (data.itemCount > 0)
                    {
                        createdItemSlots[data.slotID].SetItem(data.itemID, data.slotID, data.itemCount);
                    }
                    else
                    {
                        var destroyItemSlot = createdItemSlots[data.slotID];
                        destroyItemSlot.gameObject.SetActive(false);
                        createdItemSlots.RemoveAt(data.slotID);
                        discardItemSlots.Add(destroyItemSlot);
                    }
                }
            }
            else
            {
                if (data.itemCount > 0)
                {
                    // TODO : 새로운 인벤토리의 Visual 아이템 슬롯을 만들어준다.
                    var newItemSlot = Instantiate(itemSlotPrefab, itemSlotRoot);
                    newItemSlot.gameObject.SetActive(true);

                    Sprite itemIcon = null;
                    if (GameDataModel.Singleton.GetItemData(data.itemID, out var itemGameData))
                    {
                        itemIcon = itemGameData.ItemSprite;
                    }

                    newItemSlot.SetItem(data.itemID, data.slotID, itemIcon, data.itemCount);
                    createdItemSlots.Add(newItemSlot);
                }
            }
        }

        public void OnClickCloseButton()
        {
            InputSystem.Singleton.ChangeCursorVisibility(false);
            UIManager.Hide<InventoryMenuUI>(UIList.InventoryMenuUI);
            UIManager.Hide<InventoryUI>(UIList.InventoryUI);
        }

        public void OnNotifyOnClickItemSlot(InventoryUI_ItemSlot inventoryUI_ItemSlot)
        {
            // inventoryUI_ItemSlot.ItemSlotID
            int index = createdItemSlots.IndexOf(inventoryUI_ItemSlot);
            string itemID = createdItemSlots[index].ItemID;

            if (GameDataModel.Singleton.GetItemData(itemID, out var resultData))
            {
                UserDataModel.Singleton.UseInventoryItem(index, 1, resultData, character);
            }
        }

        public void OnNotifyOnClickItemSlot(string itemId, int useCount)
        {
            // inventoryUI_ItemSlot.ItemSlotID
            int index = createdItemSlots.FindLastIndex(x => x.ItemID.Equals(itemId));

            if (GameDataModel.Singleton.GetItemData(itemId, out var resultData))
            {
                UserDataModel.Singleton.UseInventoryItem(index, useCount, resultData, character);
            }
        }

        public void SetLinkedCharacter(CharacterBase character)
        {
            this.character = character;
        }

        public int GetItemIndex(InventoryUI_ItemSlot inventoryUI_ItemSlot)
        {
            int index = createdItemSlots.IndexOf(inventoryUI_ItemSlot);
            
            return index;
        }

        public string GetItemID(InventoryUI_ItemSlot inventoryUI_ItemSlot)
        {
            string itemID = createdItemSlots[GetItemIndex(inventoryUI_ItemSlot)].ItemID;

            return itemID;
        }
    }
}
