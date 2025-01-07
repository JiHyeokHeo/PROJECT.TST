using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public enum WeaponSocketCategory
    {
        None,
        Rifle,
        Pistol,
    }

    public enum RifleEquipCategory
    {
        None,
        M4A1,

    }

    [CreateAssetMenu(fileName = "New WeaponSocket Data", menuName = "PROJECT TST/Weapon/Weapon Socket Data")]
    public class WeaponSocketData : ScriptableObject
    {
        [field: SerializeField] public GameObject socketPrefab;

        [field: SerializeField] public WeaponSocketCategory SocketCategory { get; private set; }
        [field: SerializeField] public int RifleEquipSubCategory { get; private set; }
        
    }
}
