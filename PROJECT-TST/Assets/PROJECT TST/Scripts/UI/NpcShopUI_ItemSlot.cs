using Gpm.Ui;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TST
{
    public class NpcShop_ItemData : InfiniteScrollData
    {
        public int requirementGold;
        public ItemData itemData;
    }

    public class NpcShopUI_ItemSlot : InfiniteScrollItem
    {
        private NpcShop_ItemData npcShopItemData;

        public override void UpdateData(InfiniteScrollData scrollData)
        {
            base.UpdateData(scrollData);

            gameObject.SetActive(true);
            npcShopItemData = (NpcShop_ItemData)scrollData;

            SetItem(
                npcShopItemData.itemData.ItemID,
                npcShopItemData.requirementGold);
        }

        public void SetItem(string itemId, int requirementGold)
        {
            itemID = itemId;
            if (AssetManager.Singleton.GetItemIcon(itemID, out Sprite itemImage))
            {
                itemIcon.sprite = itemImage;
            }
            goldRequirementText.text = $" ÇÊ¿ä °ñµå : { requirementGold.ToString()}";
        }

        [SerializeField] private string itemID;

        [SerializeField] private NpcShopUI parentUI;
        [SerializeField] private Image itemIcon;
        [SerializeField] private TextMeshProUGUI goldRequirementText;

        void Update()
        {
        
        }

        public void OnClickBuyButton()
        {
            parentUI.OnNotifyOnClickBuy(npcShopItemData.itemData);
        }
    }
}
