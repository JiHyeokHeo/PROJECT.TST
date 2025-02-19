using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class PlayerEquipmentUI : UIBase
    {
        public CharacterBase character;

        public List<PlayerEquipmentUI_Slot> playerEquipmentUI_Slots = new List<PlayerEquipmentUI_Slot>();
        public void Awake()
        {
            UserDataModel.Singleton.OnPlayerEquipmentChanagedEvent += OnChangedUserEquipmentItemData;
        }

        public void OnChangedUserEquipmentItemData(PlayerEquipmentDTO.UserItemData playerEquipData)
        {

        }

        public void OnNotifyItemOnEquipment(InventoryUI_ItemData inventoryItemData)
        {
            GameManager.Instance.UseItem(inventoryItemData.itemData);
        }

        public void SetLinkedCharacter(CharacterBase character)
        {
            this.character = character;
        }

    }
}
