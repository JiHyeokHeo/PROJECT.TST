using Gpm.Ui;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TST
{
    public class InventoryUI_ItemData : InfiniteScrollData
    {
        public int itemSlotId;
        public int itemCount;
        public ItemData itemData;
    }

    public class InventoryUI_ItemSlot : InfiniteScrollItem
    {
        public int ItemSlotID => itemSlotID;
        public string ItemID => itemID;

        private InventoryUI_ItemData inventoryItemData;

        public override void UpdateData(InfiniteScrollData scrollData)
        {
            base.UpdateData(scrollData);

            gameObject.SetActive(true);
            inventoryItemData = (InventoryUI_ItemData)scrollData;

            SetItem(
                inventoryItemData.itemData.ItemID,
                inventoryItemData.itemSlotId,
                inventoryItemData.itemData.ItemSprite,
                inventoryItemData.itemCount);
        }

        public void SetItem(string itemId, int itemSlotId, int count)
        {
            itemID = itemId;
            itemSlotID = itemSlotId;
            itemCountText.text = count.ToString();
        }

        public void SetItem(string itemId, int itemSlotId, Sprite icon, int count)
        {
            itemID = itemId;
            itemSlotID = itemSlotId;
            itemIcon.sprite = icon;
            itemCountText.text = count.ToString();
        }

        [SerializeField] private int itemSlotID;
        [SerializeField] private string itemID;

        [SerializeField] private InventoryUI parentUI;
        [SerializeField] private Image itemIcon;
        [SerializeField] private TextMeshProUGUI itemCountText;


        public void Awake()
        {
        }
        private StandaloneInputModule inputModule;
        public void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (IsPointerOverMostTopGameObject())
                {
                    OnClickRightButton();
                }
            }
        }

        private bool IsPointerOverMostTopGameObject()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            if (results.Count > 0)
            {
                // 가장 위의 UI 요소만 사용
                GameObject topUIElement = results[0].gameObject;

                // 현재 객체가 가장 위에 있는 UI 요소인지 확인
                return topUIElement == gameObject;
            }

            return false;
        }

        public void OnClickItemSlot()
        {
            // Inventory UI - ItemSlot Button Click Event
            parentUI.OnNotifyOnClickItemSlot(inventoryItemData);
        }

        public void OnClickRightButton()
        {
            InventoryMenuUI inventoryMenuUI = UIManager.Show<InventoryMenuUI>(UIList.InventoryMenuUI);
            inventoryMenuUI.OnNotifyOnRightButtonClick(inventoryItemData); // 아이템 정보 전달

            InputSystem.Singleton.ChangeCursorVisibility(true);
        }
    }
}
