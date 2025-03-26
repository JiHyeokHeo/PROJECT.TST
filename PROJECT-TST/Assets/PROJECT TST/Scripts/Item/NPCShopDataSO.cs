using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    [System.Serializable]
    public class NpcItemDataBase
    {
        public string ItemID;
        public int requirementGold;
    }

    [CreateAssetMenu(fileName = "NpcShopData", menuName = "PROJECT TST/NpcShopData")]
    public class NPCShopDataSO :ScriptableObject
    {
        [field: SerializeField] public string NpcShopID { get; private set; }
        [field: SerializeField] public string NpcShopName { get; private set; }

        [field: SerializeField] public NpcItemDataBase NpcShopData { get; private set; }
    }
}
