using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TST
{
    public class MainHudUI : UIBase
    {
        public TextMeshProUGUI hpText;
        public TextMeshProUGUI weaponText;
        public TextMeshProUGUI bulletText;
        public CharacterStat characterStat;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            hpText.text = $"{characterStat.currentHp} / {characterStat.maxHp}";

            if (characterStat.currentWeapon != null)
                weaponText.text = $"{characterStat.currentWeapon.name}";
            else
                weaponText.text = $"Idle";

            bulletText.text = $"{characterStat.currentBullet} / {characterStat.maxBullet}";
        }
    }
}
