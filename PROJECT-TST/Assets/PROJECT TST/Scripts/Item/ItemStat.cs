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
        public int attackPower;
        public int defensePower;
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
        public float bulletAmount;
    }

}
