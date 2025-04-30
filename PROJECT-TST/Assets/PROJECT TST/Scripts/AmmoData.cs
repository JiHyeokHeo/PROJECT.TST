using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public enum AmmoEquipMentType
    {
        AR,
        Pistol,
        Sniper,
    }

    public enum AmmoType
    {
        Normal, // ¿œπ›≈∫
        Incendiary, // º“¿Ã≈∫
        ArmourPiercing, // √∂∞©≈∫
        End,
    }

    [CreateAssetMenu(fileName = "New Ammo Data", menuName = "PROJECT TST/Ammo/Ammo Data")]
    public class AmmoData : ScriptableObject
    {
        [field: SerializeField] public string DataID { get; private set; } 
        [field: SerializeField] public string DataName { get; private set; } 
        [field: SerializeField] public BulletProjectile AmmoVisualPrefab { get; private set; } 
        [field: SerializeField] public GameObject AmmoEffectPrefab { get; private set; } 
        [field: SerializeField] public AmmoType AmmoType { get; private set; }
        [field: SerializeField] public AmmoEquipMentType AmmoEquipmentType { get; private set; }
        [field: SerializeField] public int initAmmoCount { get; private set; }
        [field: SerializeField] public int damage { get; private set; }
    }
}
