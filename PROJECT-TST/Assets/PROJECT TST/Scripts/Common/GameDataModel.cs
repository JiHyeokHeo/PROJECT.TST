using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class GameDataModel : SingletonBase<GameDataModel>
    {
        [field: SerializeField] public List<ItemData> ItemDatas { get; private set; } = new List<ItemData>();

        public CharacterBase Character { get => mainController; set => mainController = value; }

        private CharacterBase mainController;
        public void Initialize()
        {

        }

        public bool GetItemData(string itemId, out ItemData resultData)
        {
            resultData = ItemDatas.Find(x => x.ItemID == itemId);
            return resultData != null;
        }
    }
}
