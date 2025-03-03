using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    [System.Serializable]
    public class CraftingDataBase
    {
        public string ItemID;
        public int RequireAmount;
    }
    

    [CreateAssetMenu(fileName = "CraftingData", menuName = "PROJECT TST/CraftingData")]
    public class CraftingDataSO : ScriptableObject
    {
        [field: SerializeField] public string CraftingID { get; private set; }
        [field: SerializeField] public string CraftingName { get; private set; }

        [field: SerializeField] public List<CraftingDataBase> RequireItems { get; private set; } = new List<CraftingDataBase>();

        [field: SerializeField] public string ResultItemID { get; private set; }
        [field: SerializeField] public int ResultAmount { get; private set; }
    }
}
