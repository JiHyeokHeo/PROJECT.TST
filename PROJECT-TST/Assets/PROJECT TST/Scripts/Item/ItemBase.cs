using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class ItemBase : MonoBehaviour, IInteractable
    {
        [field: SerializeField] public ItemData ItemData { get; private set; }
        public string Message => ItemData.ItemName;

        public void Interact(GameObject go)
        {
            Destroy(gameObject);

            Debug.Log("<color=#FFFFFF>Item Interacted !!</color>");

            // TODO : æ∆¿Ã≈€ »πµÊ √≥∏Æ.
            UserDataModel.Singleton.AddItemToInventory(ItemData);
        }
    }
}