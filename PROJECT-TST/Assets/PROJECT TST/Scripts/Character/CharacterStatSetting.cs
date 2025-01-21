using System.IO.Enumeration;
using UnityEngine;

namespace TST
{
    [System.Serializable]
    public class CharactStatData
    {
        public CharacterStat Base;
        public CharacterStat Max;
    }

    [CreateAssetMenu(fileName = "New Character Stat", menuName = "PROJECT TST/Character/Character Stat")]
    public class CharacterStatSetting : ScriptableObject
    {
        [field: SerializeField] public CharactStatData CharacterStatData { get; set; }
    }
}
