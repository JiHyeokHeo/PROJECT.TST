using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TST
{
    public class InventoryEquipMenuUI : UIBase, IPointerDownHandler, IDragHandler
    {
        [SerializeField] private Button useButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private TextMeshProUGUI itemNameText;

        private InventoryUI inventoryUI;
        private PlayerEquipmentUI playerEquipmentUI;
        private ItemData selectedItemData;

        private void Awake()
        {
            if (targetTransform == null)
                targetTransform = transform.Find("PopUp");
            inventoryUI = UIManager.Singleton.GetUI<InventoryUI>(UIList.InventoryUI);
            playerEquipmentUI = UIManager.Singleton.GetUI<PlayerEquipmentUI>(UIList.PlayerEquipmentUI);
        }

        public void OnNotifyOnRightButtonClick(ItemData itemData)
        {
            selectedItemData = itemData;

            // 슬롯에서부터 아이템 정보 받은 것으로 Text 및 데이터 받아오기
            itemNameText.text = itemData.ItemID; 
        }

        public void OnClickUseButton()
        {
            inventoryUI.OnNotifyOnClickItemSlot(selectedItemData.ItemID, 1);
            playerEquipmentUI.OnNotifyItemOnEquipment(selectedItemData);
            UIManager.Hide<InventoryEquipMenuUI>(UIList.InventoryEquipMenuUI);
        }

        public void OnClickCancelButton()
        {
            playerEquipmentUI.OnNotifyItemUnEquipment(selectedItemData);
            UIManager.Hide<InventoryEquipMenuUI>(UIList.InventoryEquipMenuUI);
        }

        private Transform targetTransform; // 이동될 UI

        private Vector2 startingPoint;
        private Vector2 moveBegin;
        private Vector2 moveOffset;

        // 드래그 시작 위치 지정
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            startingPoint = targetTransform.position;
            moveBegin = eventData.position;
        }

        // 드래그 : 마우스 커서 위치로 이동
        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            moveOffset = eventData.position - moveBegin;
            targetTransform.position = startingPoint + moveOffset;
        }
    }
}
