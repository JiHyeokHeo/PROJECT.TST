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
        public float crouchSpeed;
        public float runSpeed;
        public float sprintSpeed;
        public float rotateSpeed;
        public int currentBullet;
        public int maxBullet;
        public float hp;
        public float defense;
        public float damamge;
    }
}
