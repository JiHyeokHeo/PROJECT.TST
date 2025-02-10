using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TST
{
    public class InventoryUI_ItemSlot : MonoBehaviour
    {
        public int ItemSlotID => itemSlotID;
        public string ItemID => itemID;

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

        public void OnClickItemSlot()
        {
            // Inventory UI - ItemSlot Button Click Event

            parentUI.OnNotifyOnClickItemSlot(this);

        }
    }
}
