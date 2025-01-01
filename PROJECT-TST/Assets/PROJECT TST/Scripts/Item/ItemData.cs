using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public enum ItemCategory
    {
        None = 0,

        Equipment,  // 장비
        Material,   // 재료
        Consumable, // 소모품
    }

    public enum ItemEquipmentCategory
    {
        None = 0,

        Weapon,

        //Armor,
        //Shoes,
        //Shield,
    }

    public enum ItemConsumableCategory
    {
        None = 0,

        HealingKit,
    }

    [CreateAssetMenu(fileName = "New Item Data", menuName = "PROJECT TST/Item/Item Data")]
    public class ItemData : ScriptableObject
    {
        [field: SerializeField] public string ItemID { get; private set; }
        [field: SerializeField] public string ItemName { get; private set; }
        [field: SerializeField] public GameObject ItemVisualPrefab { get; private set; }
        [field: SerializeField] public Sprite ItemSprite { get; private set; }


        [field: SerializeField] public ItemCategory ItemCategory { get; private set; }
        [field: SerializeField] public int ItemSubCategory { get; private set; }
        [field: SerializeField] public int ItemMaxStack { get; private set; } = 1;
    }
}
