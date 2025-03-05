using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace TST
{
    public class PlayerEquipmentUI : UIBase
    {
        public override bool IsVisibleCursor => true;

        public CharacterBase character;
        public SerializableWrapDictionary<ItemEquipmentCategory, PlayerEquipmentUI_Slot> equipmentSlotUIs 
            = new SerializableWrapDictionary<ItemEquipmentCategory, PlayerEquipmentUI_Slot>();

        public void SetLinkedCharacter(CharacterBase character)
        {
            this.character = character;
        }

        public void Awake()
        {
            UserDataModel.Singleton.OnPlayerEquipmentChanagedEvent += OnChangedUserEquipmentItemData;
        }

        private void OnEnable()
        {
            if (UserDataModel.Singleton)
            {
                UserDataModel.Singleton.OnPlayerEquipmentChanagedEvent += OnChangedUserEquipmentItemData;
            }
        }

        private void OnDisable()
        {
            if (UserDataModel.Singleton)
            {
                UserDataModel.Singleton.OnPlayerEquipmentChanagedEvent -= OnChangedUserEquipmentItemData;
            }
        }

        public void OnChangedUserEquipmentItemData(ItemEquipmentCategory category, int beforeSlotID, int afterSlotID)
        {
            if (!equipmentSlotUIs.ContainsKey(category))
                return;

            equipmentSlotUIs[category].SetItem(afterSlotID);
        }

        public void OnNotifyItemOnEquipment(int slotId)
        {
            var targetUserItemData = UserDataModel.Singleton.UserItemData.Items.Find(x => x.slotID == slotId);
            GameDataModel.Singleton.GetItemData(targetUserItemData.itemID, out ItemData itemData);

            GameManager.Instance.UseItem(slotId, itemData);
        }

        public void OnNotifyItemUnEquipment(int slotId)
        {
            var targetUserItemData = UserDataModel.Singleton.UserItemData.Items.Find(x => x.slotID == slotId);
            GameDataModel.Singleton.GetItemData(targetUserItemData.itemID, out ItemData itemData);

            GameManager.Instance.UnEquipmentItem((ItemEquipmentCategory)itemData.ItemSubCategory, slotId);
        }

      
    }
}
