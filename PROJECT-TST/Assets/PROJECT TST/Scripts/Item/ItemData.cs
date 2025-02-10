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
        Ammo,
    }

    [CreateAssetMenu(fileName = "New Item Data", menuName = "PROJECT TST/Item/Item Data")]
    public class ItemData : ScriptableObject
    {
        [field: SerializeField] public string ItemID { get; private set; }
        [field: SerializeField] public string ItemName { get; private set; }
        [field: SerializeField] public GameObject ItemVisualPrefab { get; private set; }
        [field: SerializeField] public ItemEventHandler ItemEventHandler { get; private set; }
        [field: SerializeField] public Sprite ItemSprite { get; private set; }


        [field: SerializeField] public ItemCategory ItemCategory { get; private set; }
        [field: SerializeField] public int ItemSubCategory { get; private set; }
        [field: SerializeField] public int ItemMaxStack { get; private set; } = 1;

        [SerializeReference]
        public ItemStatBase ItemStat;
        [SerializeReference]
        public ItemStatBase ItemStatSubAdded;
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

            if (GUILayout.Button("Apply Category"))
            {
                ApplyCategory(itemData);
            }

            // 변경 사항 저장
            if (GUI.changed)
            {
                EditorUtility.SetDirty(itemData);
                serializedObject.ApplyModifiedProperties();
            };
        }

        private void ApplyCategory(ItemData itemData)
        {
            bool dataConformed = true;

            if (itemData.ItemStat != null && itemData.ItemStatSubAdded != null)
            {
                dataConformed = EditorUtility.DisplayDialog(
                    "DataChange 확인",
                    "정말로 바꾸시겠습니까?",
                    "OK",
                    "Cancel"
                );
            }

            if (dataConformed)
            {
                Undo.RecordObject(itemData, "Apply Category Change"); // Undo 지원
                switch (itemData.ItemCategory)
                {
                    case ItemCategory.Consumable:
                        itemData.ItemStat = new ConsumableStat();
                        CheckSubCategory(itemData, itemData.ItemCategory);
                        break;
                    case ItemCategory.Equipment:
                        itemData.ItemStat = new EquipmentStat();
                        CheckSubCategory(itemData, itemData.ItemCategory);
                        break;
                    case ItemCategory.Material:
                        itemData.ItemStat = new MaterialStat();
                        CheckSubCategory(itemData, itemData.ItemCategory);
                        break;
                }
            }
        }

        private void CheckSubCategory(ItemData itemData, ItemCategory type)
        {
            switch (itemData.ItemSubCategory)
            {
                case (int)ItemConsumableCategory.Ammo:
                    itemData.ItemStatSubAdded = new AmmoStat();
                    break;
                case (int)ItemConsumableCategory.None:
                    itemData.ItemStatSubAdded = null;
                    break;
            }
        }
    }
    #endregion
}
