using System.Collections;
using System.Collections.Generic;
using System.IO.Enumeration;
using UnityEngine;

namespace TST
{
    [CreateAssetMenu(fileName = "New Character Stat", menuName = "PROJECT TST/Character/Character Stat")]
    public class CharacterStat : ScriptableObject
    {
        public float walkSpeed = 1f;
        public float moveSpeed = 2f;
        public float runSpeed = 2.1f;
        public float sprintSpeed = 5f;
        public float rotateSpeed = 5f;
        public float currentBullet;
        public float maxBullet;
        public float currentHp;
        public float maxHp = 100.0f;
        public WeaponBase currentWeapon;
    }
}
