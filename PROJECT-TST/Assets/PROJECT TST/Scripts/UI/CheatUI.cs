using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace TST
{
    public class CheatUI : UIBase
    {
        public static CheatUI Instance => UIManager.Singleton.GetUI<CheatUI>(UIList.CheatUI);

        public override bool IsVisibleCursor => true;
        private List<TMP_Dropdown.OptionData> itemLists = new List<TMP_Dropdown.OptionData>();

        public TMP_Dropdown dropdown;
        public Transform characterTransform;
        public void Start()
        {
            for (int i = (int)ItemList.ITEMLIST_START + 1; i < (int)ItemList.ITEMLIST_END; i++)
            {
                TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData();
                data.text = ((ItemList)i).ToString();
                itemLists.Add(data);
            }

            characterTransform = CharacterController.Instance.transform;
            dropdown.options = itemLists;
        }

        public void OnClickGenerateItem()
        {
            GameManager.Instance.GenerateItem(characterTransform.position + characterTransform.forward * 1.0f, (ItemList)dropdown.value + 1);
        }
    }
}
