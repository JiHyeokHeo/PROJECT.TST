using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TST
{
    public class CraftingUI_Slot : UIBase
    {
        public string craftingId;

        #region MaterialA
        [Title("Material A", titleAlignment: TitleAlignments.Centered)]
        public Image materialA; // 하위 하이러키에 BackGround 관리 + 이름 + Count를 보유하고 있음
        public Image materialAGreenBackGround;
        public Image materialARedBackGround; 
        [SerializeField] private TextMeshProUGUI materialANameText;
        [SerializeField] private TextMeshProUGUI requireACountText;
        #endregion

        #region MaterialB
        [Title("Material B", titleAlignment: TitleAlignments.Centered)]
        public Image materialB;
        public Image materialBGreenBackGround;
        public Image materialBRedBackGround;
        [SerializeField] private TextMeshProUGUI materialBNameText;
        [SerializeField] private TextMeshProUGUI requireBCountText;
        #endregion

        private CraftingUI craftingUI;
        public void Awake()
        {
            craftingUI = UIManager.Singleton.GetUI<CraftingUI>(UIList.CraftingUI);

            // 이미지 설정
            if (GameDataModel.Singleton)
            {
                GameDataModel.Singleton.GetCraftingData(craftingId, out CraftingDataSO resultData);
                if (resultData.RequireItems.Count == 0)
                    return;

                // 추후 조금 수정이 있어야 하지 않을까..? 직접 접근이 뭔가 쪼매 그렇다
                string itemAId = resultData.RequireItems[0].ItemID;
                string itemBId = resultData.RequireItems[1].ItemID;
                if (AssetManager.Singleton.GetItemIcon(itemAId, out Sprite itemAImage))
                {
                    var dataA = UserDataModel.Singleton.UserItemData.GetUserItemData(itemAId);
                    materialA.sprite = itemAImage;
                    materialANameText.text = dataA.itemID;
                }

                if (AssetManager.Singleton.GetItemIcon(itemBId, out Sprite itemBImage))
                {
                    var dataB = UserDataModel.Singleton.UserItemData.GetUserItemData(itemBId);
                    materialB.sprite = itemBImage;
                    materialBNameText.text = dataB.itemID;
                }
            }
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
            GameDataModel.Singleton.GetCraftingData(craftingId, out CraftingDataSO resultData);
            if (resultData.RequireItems.Count == 0)
                return;

            // 추후 조금 수정이 있어야 하지 않을까..? 직접 접근이 뭔가 쪼매 그렇다
            string itemAId = resultData.RequireItems[0].ItemID;
            string itemBId = resultData.RequireItems[1].ItemID;

            int requireItemCount;

            #region Data A
            // 데이터 A
            var dataA = UserDataModel.Singleton.UserItemData.GetUserItemData(itemAId);
            requireItemCount = resultData.RequireItems[0].RequireAmount;
            requireACountText.text = $"{dataA.itemCount} / {requireItemCount}";

            // 백그라운드 레드 그린
            materialAGreenBackGround.gameObject.SetActive(dataA.itemCount >= requireItemCount);
            materialARedBackGround.gameObject.SetActive(dataA.itemCount < requireItemCount);
            #endregion

            #region Data B
            // 데이터 B
            var dataB = UserDataModel.Singleton.UserItemData.GetUserItemData(itemBId);
            requireItemCount = resultData.RequireItems[1].RequireAmount;
            requireBCountText.text = $"{dataB.itemCount} / {requireItemCount}";

            // 백그라운드 레드 그린
            materialBGreenBackGround.gameObject.SetActive(dataA.itemCount >= requireItemCount);
            materialBRedBackGround.gameObject.SetActive(dataA.itemCount < requireItemCount);
            #endregion
        }

        public void OnClickCraftButton()
        {
            if (craftingId != null)
                craftingUI.OnNotifyCraftingItem(craftingId);
        }
    }
}
