using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    [Serializable]
    public class SkillData 
    {
        public string skillID;
        public string skillName;
        public float cooldown;
        public Sprite skillIcon;
    }
}
