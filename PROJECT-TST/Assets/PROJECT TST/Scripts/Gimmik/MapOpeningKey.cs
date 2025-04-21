using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class MapOpeningKey : MonoBehaviour, IInteractable
    {
        public string Message => "Key Access";

        public InteractType InteractType => InteractType.Item;

        // 키에 고유한 번호를 추가한다..? (방 access 관련된 정보?)
        public int openingMapKey;

        public void Interact(GameObject go)
        {
            // 아이템 인벤토리에 Key를 추가
            GameManager.Instance.AddItem("KeyA", 1);
        }
    }
}