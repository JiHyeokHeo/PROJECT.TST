using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace TST
{
    public class ExplosiveDrum : MonoBehaviour, IDamage
    {
        public void ApplyDamage(float damage, GameObject attacker)
        {
            Debug.Log($"Damaged by {attacker.name}");
            
        }
    }
}
