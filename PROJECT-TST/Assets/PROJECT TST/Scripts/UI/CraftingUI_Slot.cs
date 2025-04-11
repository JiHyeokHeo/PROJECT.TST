using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TST
{
    public class CraftingUI_Slot : UIBase
    {
        public string itemId;
        [SerializeField] public Image image;
        [SerializeField] public GameObject greenBackground;
        [SerializeField] public GameObject redBackground;
        [SerializeField] public TextMeshProUGUI requirementCountText;

        public void SetData(string itemid)
        {
            if (AssetManager.Singleton.GetItemIcon(itemid, out Sprite itemImage))
            {
                GameDataModel.Singleton.GetItemData(itemid, out ItemData data);
                image.sprite = itemImage;
            }

            itemId = itemid;
        }

        public void UpdateSlotData(CraftDataSO resultData)
        {
            int count = UserDataModel.Singleton.UserItemData.GetUserItemDataCount(itemId);

            int possesItemCount;
            int requireItemCount;
            requireItemCount = resultData.RequireItems[0].RequireAmount;
            if (count == 0)
            {
                possesItemCount = 0;
                requirementCountText.text = $" 0 / {requireItemCount}";
            }
            else
            {
                possesItemCount = count;
                requirementCountText.text = $" {possesItemCount} / {requireItemCount}";
            }

            // 백그라운드 레드 그린
            greenBackground.gameObject.SetActive(possesItemCount >= requireItemCount);
            redBackground.gameObject.SetActive(possesItemCount < requireItemCount);

        }
    }
}
