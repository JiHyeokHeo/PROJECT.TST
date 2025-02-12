using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace TST
{
    public class HealingKit : ItemBase
    {
        protected override void RegisterItemUseAction()
        {
            if (ItemData.OnUseItem == null)
                ItemData.OnUseItem += UseItem;
        }

        public override void UseItem(CharacterBase user)
        {
            ItemStatBase stat = ItemData.ItemStat;

            if (stat == null)
            {
                Assert.IsNull(stat, $"{stat} is Not Valid");
                return;
            }

            Debug.Log("HealingKit Used");
            if (stat is ConsumableStat consumableStat)
            {
                user.CurrentHp += consumableStat.buffValue;
            }
        }
    }
}
