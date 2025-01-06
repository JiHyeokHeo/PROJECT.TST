using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TST
{
    public class InteractionUI : UIBase
    {
        public static InteractionUI Instance => UIManager.Singleton.GetUI<InteractionUI>(UIList.InteractionUI);

        public void ShowInteractionItem()
        {
            // TODO : itemRoot를 활성화한다.
            // TODO : itemNameText를 설정한다.
            itemRoot.gameObject.SetActive(true);

        }

        public void HideInteractionItem()
        {
            // TODO : itemRoot를 비 활성화한다.
            // TODO : itemNameText를 초기화한다.
            itemRoot.gameObject.SetActive(false);

        }

        [SerializeField] private GameObject itemRoot;
        [SerializeField] private TextMeshProUGUI nameText;

        private void Awake()
        {
            itemRoot.gameObject.SetActive(false);
        }
    }
}
