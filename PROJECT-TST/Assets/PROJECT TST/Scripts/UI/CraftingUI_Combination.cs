using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TST
{
    public class CraftingUI_Combination : UIBase
    {
        public string craftingId;
        [SerializeField] private CraftingUI craftingUI;
        public CraftingUI_Slot craftingUI_Slot_Prefab;

        private List<CraftingUI_Slot> craftingUI_Slots = new List<CraftingUI_Slot>();

        public Transform contentRootTransform;
        public Transform slotRootTransform;
        public Image resultUISlotImage;
        public TextMeshProUGUI resultItemName;
        public TextMeshProUGUI resultRequirementText;

        public void Start()
        {
            // 이미지 설정

            bool isEmpty = craftingId.Equals("");
            //Assert.IsFalse(isEmpty, "crafting id is Empty");
            if (isEmpty)
                return;

            if (GameDataModel.Singleton)
            {
                GameDataModel.Singleton.GetCraftingData(craftingId, out CraftDataSO resultData);
                if (resultData.RequireItems.Count == 0)
                    return;

                int requireItemCounts = resultData.RequireItems.Count;

                for (int i = 0; i < requireItemCounts; i++)
                {
                    string itemId = resultData.RequireItems[i].ItemID;
                    if (AssetManager.Singleton.GetItemIcon(itemId, out Sprite itemImage))
                    {
                        GameDataModel.Singleton.GetItemData(itemId, out ItemData data);

                        CraftingUI_Slot slotObject = Instantiate(craftingUI_Slot_Prefab, slotRootTransform);
                        slotObject.gameObject.SetActive(true);
                        craftingUI_Slots.Add(slotObject);
                        slotObject.SetData(itemId);
                    }
                }

                if (AssetManager.Singleton.GetItemIcon(resultData.ResultItemID, out Sprite resultImage))
                {
                    resultUISlotImage.sprite = resultImage;
                    resultItemName.text = resultData.ResultItemID;
                    resultRequirementText.text = $"제작 : {resultData.ResultAmount}";
                }

            }

            UpdateMaterialDatas();
        }

        public void OnEnable()
        {
            if (UserDataModel.Singleton)
                UserDataModel.Singleton.OnUserItemChangedEvent += _ => UpdateMaterialDatas();
        }

        public void OnDisable()
        {
            if (UserDataModel.Singleton)
                UserDataModel.Singleton.OnUserItemChangedEvent -= _ => UpdateMaterialDatas();
        }

        // 메테리얼 데이터 업데이트 // 이걸 추가시켜야할듯 GameManager에다가
        public void UpdateMaterialDatas()
        {
            if (string.IsNullOrEmpty(craftingId))
                return;

            GameDataModel.Singleton.GetCraftingData(craftingId, out CraftDataSO resultData);
            if (resultData.RequireItems.Count <= 0)
                return;

            for (int i = 0; i < craftingUI_Slots.Count; i++)
            {
                craftingUI_Slots[i].UpdateSlotData(resultData);
            }
        }

        public void OnClickCraftButton()
        {
            if (craftingId != null)
                craftingUI.OnNotifyCraftingItem(craftingId);
        }

        public void SetCraftingID(string id)
        {
            craftingId = id;
        }
    }
}
