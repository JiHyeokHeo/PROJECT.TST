using Gpm.Ui;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TST
{
    public class PlayerEquipmentUI_Slot : MonoBehaviour
    {
        public int SlotID => slotid;

        [SerializeField] private ItemData itemData;

        [SerializeField] private int slotid;
        [SerializeField] private Image equipmentIcon;
        [SerializeField] private TextMeshProUGUI equipmentNameText;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (IsPointerOverMostTopGameObject())
                {
                    OnClickRightButton();
                }
            }
        }

        public void SetItem(int slotId)
        {
            this.slotid = slotId;   
            gameObject.SetActive(slotId >= 0);
            if (slotid >= 0)
            {
                var targetUserItemData = UserDataModel.Singleton.UserItemData.Items.Find(x => x.slotID == slotId);
                GameDataModel.Singleton.GetItemData(targetUserItemData.itemID, out this.itemData);
                if (AssetManager.Singleton.GetItemIcon(itemData.ItemID, out Sprite iconImage))
                {
                    equipmentIcon.sprite = iconImage;

                }
                equipmentNameText.text = itemData.ItemID;
            }
            else
            {
                equipmentIcon.sprite = null;
                equipmentNameText.text = string.Empty;
            }
        }

        private bool IsPointerOverMostTopGameObject()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            if (results.Count > 0)
            {
                // 가장 위의 UI 요소만 사용
                GameObject topUIElement = results[0].gameObject;

                // 현재 객체가 가장 위에 있는 UI 요소인지 확인
                return topUIElement == gameObject;
            }

            return false;
        }

        public void OnClickRightButton()
        {
            if (itemData.ItemCategory == ItemCategory.Equipment)
            {
                InventoryEquipMenuUI invenEquipmentUI = UIManager.Show<InventoryEquipMenuUI>(UIList.InventoryEquipMenuUI);
                invenEquipmentUI.OnNotifyOnRightButtonClick(SlotID);
            }
           
            InputSystem.Singleton.ChangeCursorVisibility(true);
        }
    }
}
