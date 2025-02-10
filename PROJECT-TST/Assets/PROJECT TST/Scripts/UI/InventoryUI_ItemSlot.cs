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

        public void SetItem(int itemSlotId, int count)
        {
            itemSlotID = itemSlotId;
            itemCountText.text = count.ToString();
        }

        public void SetItem(int itemSlotId, Sprite icon, int count)
        { 
            itemSlotID = itemSlotId;
            itemIcon.sprite = icon;
            itemCountText.text = count.ToString();
        }

        [SerializeField] private int itemSlotID;

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
