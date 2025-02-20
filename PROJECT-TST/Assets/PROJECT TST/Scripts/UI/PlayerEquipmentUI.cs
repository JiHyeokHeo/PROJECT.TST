using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace TST
{
    public class PlayerEquipmentUI : UIBase
    {
        public CharacterBase character;

        public List<PlayerEquipmentUI_Slot> playerEquipmentUI_Slots = new List<PlayerEquipmentUI_Slot>();
        public void Awake()
        {
            UserDataModel.Singleton.OnPlayerEquipmentChanagedEvent += OnChangedUserEquipmentItemData;

            //PlayerEquipmentUI_Slot[] components =  GetComponentsInChildren<PlayerEquipmentUI_Slot>();

            //for (int i = 0; i < components.Length; i++)
            //    playerEquipmentUI_Slots.Add(components[i]);
        }

        public void OnChangedUserEquipmentItemData(PlayerEquipmentDTO.UserItemData playerEquipData, ItemData itemData)
        {
            Debug.Log("playerEquipmentUI Event ¹ß»ý");
            int index = playerEquipData.equipUIslotID;

            playerEquipmentUI_Slots[index].SetItem(itemData.ItemID, index, itemData.ItemSprite, itemData);
        }

        public void OnNotifyItemOnEquipment(ItemData itemdata)
        {
            GameManager.Instance.UseItem(itemdata);
        }

        public void OnNotifyItemUnEquipment(ItemData itemdata)
        {
            GameManager.Instance.UnEquipmentItem(itemdata);
        }

        public void SetLinkedCharacter(CharacterBase character)
        {
            this.character = character;
        }

    }
}
