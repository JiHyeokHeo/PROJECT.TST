using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class IngameUI : UIBase
    {
        public void OnClickShowInventoryUIButton()
        {
            UIManager.Show<InventoryUI>(UIList.InventoryUI);
        }

        public void OnClickShowEquipmentUIButton()
        {
            UIManager.Show<PlayerEquipmentUI>(UIList.PlayerEquipmentUI);
        }

        public void OnClickShowCraftingUIButton()
        {
            UIManager.Show<CraftingUI>(UIList.CraftingUI);
        }

        public void OnClickShowSettingUIButton()
        {

        }
    }
}
