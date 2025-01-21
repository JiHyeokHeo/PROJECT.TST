using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    [Serializable]
    public struct CharacterStat
    {
        public float walkSpeed;
        public float moveSpeed;
        public float runSpeed;
        public float sprintSpeed;
        public float rotateSpeed;
        public float currentBullet;
        public float maxBullet;
        public float hp;
    }
}
