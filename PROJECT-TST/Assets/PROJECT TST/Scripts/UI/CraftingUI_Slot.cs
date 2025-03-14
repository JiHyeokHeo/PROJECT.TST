using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UI;

namespace TST
{
    public class CraftingUI_Slot : UIBase
    {
        public string craftingId;
        [SerializeField] private CraftingUI craftingUI;

        #region MaterialA
        [Title("Material A", titleAlignment: TitleAlignments.Centered)]
        [SerializeField] public Image materialAIcon; // 하위 하이러키에 BackGround 관리 + 이름 + Count를 보유하고 있음
        [SerializeField] public Image materialAGreenBackGroundIcon;
        [SerializeField] public Image materialARedBackGroundIcon; 
        [SerializeField] private TextMeshProUGUI materialANameText;
        [SerializeField] private TextMeshProUGUI requireACountText;
        #endregion

        #region MaterialB
        [Title("Material B", titleAlignment: TitleAlignments.Centered)]
        [SerializeField] public Image materialBIcon;
        [SerializeField] public Image materialBGreenBackGroundIcon;
        [SerializeField] public Image materialBRedBackGroundIcon;
        [SerializeField] private TextMeshProUGUI materialBNameText;
        [SerializeField] private TextMeshProUGUI requireBCountText;
        #endregion

        [Title("Result", titleAlignment: TitleAlignments.Centered)]
        [SerializeField] public Image resultItem;
        [SerializeField] public Image resultItemGreenBackGround;
        [SerializeField] public Image resultItemRedBackGround;
        [SerializeField] private TextMeshProUGUI resultItemNameText;
        [SerializeField] private TextMeshProUGUI resultItemCountText;

        public void Awake()
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

                // 추후 조금 수정이 있어야 하지 않을까..? 직접 접근이 뭔가 쪼매 그렇다
                string itemAId = resultData.RequireItems[0].ItemID;
                string itemBId = resultData.RequireItems[1].ItemID;
                if (AssetManager.Singleton.GetItemIcon(itemAId, out Sprite itemAImage))
                {
                    GameDataModel.Singleton.GetItemData(itemAId, out ItemData dataA);
                    materialAIcon.sprite = itemAImage;
                    materialANameText.text = dataA.ItemName;
                }

                if (AssetManager.Singleton.GetItemIcon(itemBId, out Sprite itemBImage))
                {
                    GameDataModel.Singleton.GetItemData(itemBId, out ItemData dataB);
                    materialBIcon.sprite = itemBImage;
                    materialBNameText.text = dataB.ItemName;
                }

                if (AssetManager.Singleton.GetItemIcon(resultData.ResultItemID, out Sprite itemResultImage))
                {
                    GameDataModel.Singleton.GetItemData(resultData.ResultItemID, out ItemData result);
                    resultItem.sprite = itemResultImage;
                    resultItemNameText.text = result.ItemName;
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
            if (craftingId == "")
                return;

            GameDataModel.Singleton.GetCraftingData(craftingId, out CraftDataSO resultData);
            if (resultData.RequireItems.Count == 0)
                return;

            // 추후 조금 수정이 있어야 하지 않을까..? 직접 접근이 뭔가 쪼매 그렇다
            string itemAId = resultData.RequireItems[0].ItemID;
            string itemBId = resultData.RequireItems[1].ItemID;

            int possesItemCount;
            int requireItemCount;
            #region Data A
            // 데이터 A
            int count = UserDataModel.Singleton.UserItemData.GetUserItemDataCount(itemAId);
            requireItemCount = resultData.RequireItems[0].RequireAmount;
            if (count == 0)
            {
                possesItemCount = 0;
                requireACountText.text = $"0 / {requireItemCount}";
            }
            else
            {
                possesItemCount = count;
                requireACountText.text = $"{possesItemCount} / {requireItemCount}";
            }

            // 백그라운드 레드 그린
            materialAGreenBackGroundIcon.gameObject.SetActive(possesItemCount >= requireItemCount);
            materialARedBackGroundIcon.gameObject.SetActive(possesItemCount < requireItemCount);
            #endregion

            #region Data B
            // 데이터 B
            count = UserDataModel.Singleton.UserItemData.GetUserItemDataCount(itemBId);
            requireItemCount = resultData.RequireItems[1].RequireAmount;
            if (count == 0)
            {
                possesItemCount = 0;
                requireBCountText.text = $"0 / {requireItemCount}";
            }
            else
            {
                possesItemCount = count;
                requireBCountText.text = $"{possesItemCount} / {requireItemCount}";
            }

            // 백그라운드 레드 그린
            materialBGreenBackGroundIcon.gameObject.SetActive(possesItemCount >= requireItemCount);
            materialBRedBackGroundIcon.gameObject.SetActive(possesItemCount < requireItemCount);
            #endregion

            if (GameDataModel.Singleton.GetItemData(resultData.ResultItemID, out ItemData resultItemData))
            {
                resultItemCountText.text = $"{resultData.ResultAmount}" ;
            }


            resultItemGreenBackGround.gameObject.SetActive (
                materialAGreenBackGroundIcon.IsActive() && materialBGreenBackGroundIcon.IsActive()
                );

            resultItemRedBackGround.gameObject.SetActive(!resultItemGreenBackGround.IsActive());
        }

        public void OnClickCraftButton()
        {
            if (craftingId != null)
                craftingUI.OnNotifyCraftingItem(craftingId);
        }
    }
}
