using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public abstract class CharacterSkillBase 
    {
        public SkillData SkillData => skillData;

        protected SkillData skillData;

        public CharacterSkillBase(SkillData data)
        {
            skillData = data;
        }

        public abstract void OnExecute(CharacterBase actor);

        public abstract void OnCancel(CharacterBase actor);
    }
}
