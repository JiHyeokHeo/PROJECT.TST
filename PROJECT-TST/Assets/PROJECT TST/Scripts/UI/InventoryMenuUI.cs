using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TST
{
    public class InventoryMenuUI : UIBase
    {
        [field : SerializeField] public Transform MenuRoot { get; private set; }
        [field : SerializeField] public TMP_InputField InputField { get; private set; }

        [SerializeField] private Button useButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private TextMeshProUGUI itemNameText;

        private InventoryUI inventoryUI;

        private InventoryUI_ItemSlot itemSlot;
        private string selectedItemId;
        private void Awake()
        {
            inventoryUI = UIManager.Singleton.GetUI<InventoryUI>(UIList.InventoryUI);
        }

        public void Update()
        {


        }

        public void OnNotifyOnRightButtonClick(InventoryUI_ItemSlot itemSlot)
        {
            MenuRoot.gameObject.SetActive(true);

            // 슬롯에서부터 아이템 정보 받은 것으로 Text 및 데이터 받아오기
            this.itemSlot = itemSlot;
            selectedItemId = inventoryUI.GetItemID(itemSlot);
            itemNameText.text = selectedItemId;
        }

        public void OnClickUseButton()
        {
            inventoryUI.OnNotifyOnClickItemSlot(itemSlot, int.Parse(InputField.text));
        }

        public void OnClickCancelButton()
        {
            MenuRoot.gameObject.SetActive(false);
        }
    }
}
