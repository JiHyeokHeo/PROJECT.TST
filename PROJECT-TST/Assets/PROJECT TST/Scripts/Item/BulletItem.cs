using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace TST
{
    public class BulletItem : ItemBase
    {
        protected override void RegisterItemUseAction()
        {
            if (ItemData.OnUseItem == null)
                ItemData.OnUseItem += UseItem;
        }

        public override void UseItem(CharacterBase user)
        {
            ItemStatBase stat = ItemData.ItemStatSubAdded;

            if (stat == null)
            {
                Assert.IsNull(stat, $"{stat} is Not Valid");
                return;
            }

            if (stat is AmmoStat consumableStat)
            {
                int existRifleIndex = user.rifleAmmos.FindLastIndex(x => x.data.AmmoType.Equals(consumableStat.AmmoType));
                int existPistolIndex = user.pistolAmmos.FindLastIndex(x => x.data.AmmoType.Equals(consumableStat.AmmoType));
                if (existRifleIndex >= 0)
                {
                    user.rifleAmmos[existRifleIndex].CurrentAmmo += consumableStat.bulletAmount;
                    user.primaryWeapon.AddMaxAmountBullet(consumableStat.bulletAmount);
                }

                if (existPistolIndex >= 0) 
                {
                    user.pistolAmmos[existPistolIndex].CurrentAmmo += consumableStat.bulletAmount;
                    user.subWeapon.AddMaxAmountBullet(consumableStat.bulletAmount);
                }
            }
        }
    }
}
