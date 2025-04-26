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
        [SerializeField] private Image backgroundIcon;
        [SerializeField] private TextMeshProUGUI equipmentNameText;

        // Start is called before the first frame update
        void Start()
        {
            SetItem(-1);
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
            if (slotid >= 0)
            {
                gameObject.SetActive(slotId >= 0);  
                var targetUserItemData = UserDataModel.Singleton.UserItemData.Items.Find(x => x.slotID == slotId);
                GameDataModel.Singleton.GetItemData(targetUserItemData.itemID, out this.itemData);
                if (AssetManager.Singleton.GetItemIcon(itemData.ItemID, out Sprite iconImage))
                {
                    equipmentIcon.sprite = iconImage;

                }
                equipmentNameText.text = itemData.ItemID;
                equipmentIcon.color = new Color(equipmentIcon.color.r, equipmentIcon.color.g, equipmentIcon.color.b, 1f);
                backgroundIcon.color = new Color(0f, 255f, 0f, 0.3f); ;
            }
            else
            {
                equipmentIcon.sprite = null;
                equipmentIcon.color = new Color(equipmentIcon.color.r, equipmentIcon.color.g, equipmentIcon.color.b, 0f);
                equipmentNameText.text = string.Empty;
                backgroundIcon.color = new Color(255f, 0f, 0f, 0.3f);
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
