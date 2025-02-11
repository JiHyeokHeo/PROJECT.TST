using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class HealingKit : ItemBase
    {
        public override void UseItem(CharacterBase user)
        {
            Debug.Log("HealingKit Used");
        }
    }
}
