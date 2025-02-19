using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class ItemBase : MonoBehaviour, IInteractable
    {
        [field: SerializeField] public ItemData ItemData { get; private set; }
        public string Message => ItemData.ItemName;

        public void Awake()
        {
            
        }

        public void Interact(GameObject go)
        {
            Destroy(gameObject);

            Debug.Log("<color=#FFFFFF>Item Interacted !!</color>");

            UserDataModel.Singleton.AddItemToInventory(ItemData);
        }

        public virtual void UseItem(CharacterBase user)
        {
            
        }
    }
}