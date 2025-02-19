using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    [Serializable]
    public class ItemStatBase
    {

    }

    [Serializable]
    public class EquipmentStat : ItemStatBase
    {
        public float attackPower;
        public float defensePower;
        public float shield;
    }

    [Serializable]
    public class MaterialStat : ItemStatBase
    {
        public int craftingValue;
    }

    [Serializable]
    public class ConsumableStat : ItemStatBase
    {
        public float buffValue;
        public float debuffValue;
    }

    [Serializable]
    public class AmmoStat : ItemStatBase
    {
        public int bulletAmount;
        [field: SerializeField] public AmmoType AmmoType { get; private set; }
        [field: SerializeField] public AmmoEquipMentType AmmoEquipmentType { get; private set; }
    }

}
