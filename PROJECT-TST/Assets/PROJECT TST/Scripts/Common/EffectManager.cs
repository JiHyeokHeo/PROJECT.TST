using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

namespace TST
{
    public enum EffectType
    {
        Muzzle_1,
        Muzzle_6,
        Muzzle_9,


        Impact_Brick,
        Impact_Dirt,
        Impact_Metal,
    }

    [System.Serializable]
    public class EffectData
    {
        public EffectType type;
        public GameObject prefab;
        public float duration;
    }

    public class EffectManager : MonoBehaviour
    {
        public static EffectManager Instance { get; private set; }

        public List<EffectData> effectContainer = new List<EffectData>();

        private void Awake()
        {
            Instance = this;
        }

        public GameObject SpawnEffect(EffectType type)
        {
            EffectData data = effectContainer.Find(x => x.type == type);
            if (data == null)
            {
                Debug.LogError("Effect not found");
                return null;
            }
            
            GameObject effect = Instantiate(data.prefab);
            effect.gameObject.SetActive(true);

            Destroy(effect, data.duration);

            return effect;
        }
    }
}
