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
        [field : SerializeField] public Transform MenuRoot { get; private set; }

        [SerializeField] private Button useButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private TextMeshProUGUI itemNameText;

        private InventoryUI inventoryUI;
        private InventoryUI_ItemData selectedItemData;

        private void Awake()
        {
            if (targetTransform == null)
                targetTransform = transform.Find("PopUp");
            inventoryUI = UIManager.Singleton.GetUI<InventoryUI>(UIList.InventoryUI);
        }

        public void OnNotifyOnRightButtonClick(InventoryUI_ItemData inventoryItemData)
        {
            MenuRoot.gameObject.SetActive(true);
            selectedItemData = inventoryItemData;

            // 슬롯에서부터 아이템 정보 받은 것으로 Text 및 데이터 받아오기
            itemNameText.text = selectedItemData.itemData.ItemID; 
        }

        public void OnClickUseButton()
        {
            inventoryUI.OnNotifyOnClickItemSlot(selectedItemData.itemData.ItemID, 1);
        }

        public void OnClickCancelButton()
        {
            MenuRoot.gameObject.SetActive(false);
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
