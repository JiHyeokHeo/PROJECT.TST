using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public interface IDamage  
    {
        public void ApplyDamage(float damage, GameObject owner);
    }
}
