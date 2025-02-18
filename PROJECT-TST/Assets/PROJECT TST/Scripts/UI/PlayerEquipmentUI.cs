using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class PlayerEquipmentUI : UIBase
    {
        public CharacterBase character;

        public void Awake()
        {
            
        }

        private void OnEnable()
        {

        }

        public void OnNotifyItemOnEquipment(InventoryUI_ItemData inventoryItemData)
        {
            GameManager.Instance.UseItem(inventoryItemData.itemData);
        }

        public void LinkCharacter(CharacterBase character)
        {
            this.character = character;
        }

        
    }
}
