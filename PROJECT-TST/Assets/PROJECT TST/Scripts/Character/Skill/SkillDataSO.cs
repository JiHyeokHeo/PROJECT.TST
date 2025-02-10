using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    [CreateAssetMenu(fileName = "SkillData", menuName = "PROJECT TST/SkillDataSO")]
    public class SkillDataSO : ScriptableObject
    {
        [field: SerializeField] public SkillData SkillData { get; set;}

    }
}
