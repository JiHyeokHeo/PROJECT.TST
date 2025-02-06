using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
        [SerializeReference]
        public ItemStatBase ItemStat;
    }

    #region ItemDataEditor
    // 에디터 관련
    [CustomEditor(typeof(ItemData))]
    public class ItemDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            ItemData itemData = (ItemData)target;

            // 기본 Inspector UI
            DrawDefaultInspector();

            // 카테고리에 따라 ItemStat을 변경
            if (GUILayout.Button("Apply Category"))
            {
                switch (itemData.ItemCategory)
                {
                    case ItemCategory.Consumable:
                        itemData.ItemStat = new ConsumableStat();
                        break;
                    case ItemCategory.Equipment:
                        itemData.ItemStat = new EquipmentStat();
                        break;
                    case ItemCategory.Material:
                        itemData.ItemStat = new MaterialStat();
                        break;
                }
            }

            // 변경 사항 저장
            EditorUtility.SetDirty(itemData);
        }
    }
    #endregion
}
