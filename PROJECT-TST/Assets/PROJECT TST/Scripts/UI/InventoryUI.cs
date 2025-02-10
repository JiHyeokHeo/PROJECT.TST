using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class InventoryUI : UIBase
    {

        [SerializeField] private Transform itemSlotRoot;
        [SerializeField] private InventoryUI_ItemSlot itemSlotPrefab;

        private List<InventoryUI_ItemSlot> createdItemSlots = new List<InventoryUI_ItemSlot>();

        private void Awake()
        {
            UserDataModel.Singleton.OnUserItemChangedEvent += OnChangedUserItemData;
            itemSlotPrefab.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            // TODO : UserDataModel의 UserItemData에 있는 아이템 데이터를 읽고, 인벤토리에 표기해준다.
        }

        private void OnDisable()
        {
            
        }

        private void OnChangedUserItemData(UserItemDTO.UserItemData data)
        {
            // TODO : 인벤토리에 표기하는 아이템들은 Dictionary<int, InventoryUI_ItemSlot> createdItemSlots 에 저장하고 관리한다.
            // TODO : Dictionary<int, InventoryUI_ItemSlot> 의 int Key 값은 SlotID 와 동일하다.

            if (createdItemSlots.Exists(x=>x.ItemSlotID == data.slotID))
            {
                int index = createdItemSlots.FindIndex(x=>x.ItemSlotID == data.slotID);
                if (index>=0)
                {
                    createdItemSlots[data.slotID].SetItem(data.slotID, data.itemCount);
                }
            }
            else
            {
                // TODO : 새로운 인벤토리의 Visual 아이템 슬롯을 만들어준다.
                var newItemSlot = Instantiate(itemSlotPrefab, itemSlotRoot);
                newItemSlot.gameObject.SetActive(true);

                Sprite itemIcon = null;
                if (GameDataModel.Singleton.GetItemData(data.itemID, out var itemGameData))
                {
                    itemIcon = itemGameData.ItemSprite;
                }

                newItemSlot.SetItem(data.slotID, itemIcon, data.itemCount);
                createdItemSlots.Add(newItemSlot);
            }
        }

        public void OnClickCloseButton()
        {
            UIManager.Hide<InventoryUI>(UIList.InventoryUI);
        }

        public void OnNotifyOnClickItemSlot(InventoryUI_ItemSlot inventoryUI_ItemSlot)
        {
            // inventoryUI_ItemSlot.ItemSlotID
            int index = createdItemSlots.IndexOf(inventoryUI_ItemSlot);
        }
    }
}
