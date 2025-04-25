using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TST
{
    public class InteractionUI : UIBase
    {
        public static InteractionUI Instance
        {
            get
            {
                return UIManager.Show<InteractionUI>(UIList.InteractionUI);
            }
            private set { }
        }

        public void ShowInteraction(IInteractable interactable)
        {
            // TODO : itemRoot를 활성화한다.
            // TODO : itemNameText를 설정한다.
            if (interactable is ItemBase itemBase)
            {
                itemRoot.gameObject.SetActive(true);
                nameText.text = itemBase.ItemData.ItemName;
            }
            else if (interactable is NPCController npc)
            {
                itemRoot.gameObject.SetActive(true);
                nameText.text = $"{npc.NpcName} Talk" ;
            }
            else
            {
                itemRoot.gameObject.SetActive(true);
                nameText.text = interactable.Message;
            }
            
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
            Instance = this;
            itemRoot.gameObject.SetActive(false);
        }
    }
}
