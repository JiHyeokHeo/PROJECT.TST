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
