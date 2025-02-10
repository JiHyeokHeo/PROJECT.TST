using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class CharacterSkill_SlingShot : CharacterSkillBase
    {
        public CharacterSkill_SlingShot(SkillData data) : base(data)
        {

        }

        public override void OnExecute(CharacterBase actor)
        {
            // 현재 이 스킬을 썼을 때 실행 되어야 하는 로직을 작성
            
        }

        public override void OnCancel(CharacterBase actor)
        {
            // 현재 이 스킬을 취소 시켰을 때 실행 되어야 하는 로직을 작성
        }
    }
}
