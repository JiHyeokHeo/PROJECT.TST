using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class CraftingUI : UIBase
    {
        // 너가 조합식을 다 가지고 있자..
        public override bool IsVisibleCursor => true;
        public CraftingUI_Combination craftingSlotObject;

        public void Awake()
        {
            for (int i = 0; i <  GameDataModel.Singleton.CraftingDatas.Count; i++)
            {
                var slotObj = Instantiate(craftingSlotObject.gameObject, craftingSlotObject.contentRootTransform);
                slotObj.SetActive(true);
                CraftingUI_Combination script = slotObj.GetComponent<CraftingUI_Combination>();
                string Id = GameDataModel.Singleton.CraftingDatas[i].CraftingID;
                script.SetCraftingID(Id);
            }
        }

        // crafting 버튼마다 고유한 crafting result 정보를 가지고 있는다?
        public void OnNotifyCraftingItem(string crafting_id)
        {
            GameManager.Instance.CraftItem(crafting_id);
        }

        public void OnCloseButton()
        {
            UIManager.Hide<CraftingUI>(UIList.CraftingUI);
        }
    }
}
