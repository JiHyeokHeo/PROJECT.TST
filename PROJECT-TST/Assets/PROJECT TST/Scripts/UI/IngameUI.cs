using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class IngameUI : UIBase
    {
        public void OnClickInventoryButton()
        {
            UIManager.Show<InventoryUI>(UIList.InventoryUI);
        }
    }
}
