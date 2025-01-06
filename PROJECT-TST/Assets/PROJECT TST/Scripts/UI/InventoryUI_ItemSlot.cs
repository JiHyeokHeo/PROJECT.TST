using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TST
{
    public class InventoryUI_ItemSlot : MonoBehaviour
    {
        public void SetItem(int count)
        {
            itemCountText.text = count.ToString();
        }

        public void SetItem(Sprite icon, int count)
        { 
            itemIcon.sprite = icon;
            itemCountText.text = count.ToString();
        }

        [SerializeField] private Image itemIcon;
        [SerializeField] private TextMeshProUGUI itemCountText;
    }
}
