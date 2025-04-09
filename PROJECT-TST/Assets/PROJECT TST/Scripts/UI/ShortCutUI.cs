using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TST
{
    public class ShortCutUI : UIBase
    {
        [SerializeField] public Image greenBackground;
        [SerializeField] public Image redBackground;
        [SerializeField] public TextMeshProUGUI itemCountText;
        [SerializeField] public Image itemImage;
        [field: SerializeField] public string ItemID { get; set; }

        public void Start()
        {
            SetShortCutImageData();
            UpdateShortCutData();
        }

        public void OnEnable()
        {
            if (UserDataModel.Singleton)
                UserDataModel.Singleton.OnUserItemChangedEvent += _ => UpdateShortCutData();
        }

        public void OnDisable()
        {
            if (UserDataModel.Singleton)
                UserDataModel.Singleton.OnUserItemChangedEvent -= _ => UpdateShortCutData();
        }

        public void SetShortCutImageData()
        {
            // 유저데이터 모델을 일단 들고오자
            if (string.IsNullOrEmpty(ItemID) == false)
            {
                if (AssetManager.Singleton.GetItemIcon(ItemID, out Sprite itemImage))
                {
                    this.itemImage.sprite = itemImage;
                }
            }
        }

        public void UpdateShortCutData()
        {
            if (string.IsNullOrEmpty(ItemID) == false)
            {
                int itemCount = UserDataModel.Singleton.UserItemData.GetUserItemDataCount(ItemID);
                itemCountText.text = $"수량: {itemCount}";

                greenBackground.gameObject.SetActive(itemCount > 0);
                redBackground.gameObject.SetActive(itemCount <= 0);
            }
        }

        public void UseShortCutItem()
        {
            if (GameDataModel.Singleton.GetItemData(ItemID, out var itemData))
            {
                GameManager.Instance.UseItem(-1, itemData);
            }
        }
    }
}
