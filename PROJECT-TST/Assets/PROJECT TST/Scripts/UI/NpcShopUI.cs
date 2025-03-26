using Gpm.Ui;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace TST
{
    public class NpcShopUI : UIBase
    {
        public override bool IsVisibleCursor => true;


        [SerializeField] private Transform itemSlotRoot;
        [SerializeField] private NpcShopUI_ItemSlot itemSlotPrefab;
        [SerializeField] private InfiniteScroll infiniteScroll;


        [SerializeField] private TextMeshProUGUI GoldText;
        private void Awake()
        {
            itemSlotPrefab.gameObject.SetActive(false);

            int maxCount = GameDataModel.Singleton.NpcShopDatas.Count;

            for (int i =0; i < maxCount; i++)
            {
                NpcShop_ItemData shoppingData = new NpcShop_ItemData();

                var itemSO =  GameDataModel.Singleton.NpcShopDatas[i];
                if (GameDataModel.Singleton.GetItemData(itemSO.NpcShopData.ItemID, out var itemGameData))
                {
                    shoppingData.requirementGold = itemSO.NpcShopData.requirementGold;
                    shoppingData.itemData = itemGameData;
                }

                infiniteScroll.InsertData(shoppingData);
            }
        }

        public void OnEnable()
        {
            if (UserDataModel.Singleton)
            {
                UserDataModel.Singleton.OnUserItemChangedEvent += _ => OnMoneyDataChanged();
                OnMoneyDataChanged();
            }

        }

        public void OnDisable()
        {
            if (UserDataModel.Singleton)
            {
                UserDataModel.Singleton.OnUserItemChangedEvent -= _ => OnMoneyDataChanged();
            }
        }

        public void OnMoneyDataChanged()
        {
            int moneyAmount = UserDataModel.Singleton.UserItemData.GetUserItemDataCount("Money");
            if (moneyAmount <= 0) 
                moneyAmount = 0;

            GoldText.text = $"{moneyAmount.ToString()} / 2147483647";
        }

        public void OnClickCloseButton()
        {
            InputSystem.Singleton.ChangeCursorVisibility(false);
            UIManager.Hide<NpcShopUI>(UIList.NpcShopUI);
        }

        public void OnNotifyOnClickBuy(ItemData itemData, int useCount = 1)
        {
            NpcShop_ItemData targetNpcItemData = null;
            var inventoryDataList = infiniteScroll.GetDataList();
            for (int i = 0; i < inventoryDataList.Count; i++)
            {
                var castingData = inventoryDataList[i] as NpcShop_ItemData;
                if (castingData.itemData == itemData)
                {
                    targetNpcItemData = castingData;
                    break;
                }
            }

            if (targetNpcItemData != null)
            {
                GameManager.Instance.AddItem(targetNpcItemData.itemData.ItemID, useCount);

                if (GameDataModel.Singleton.GetItemData("Money", out ItemData resultData))
                {
                    GameManager.Instance.UseItem(-1, resultData, targetNpcItemData.requirementGold);
                }
            }
        }

    }
}
