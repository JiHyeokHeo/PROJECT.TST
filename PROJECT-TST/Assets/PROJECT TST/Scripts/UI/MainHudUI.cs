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
        public CharacterBase linkedCharacter;

        // Start is called before the first frame update
        void Start()
        {
            linkedCharacter = GameDataModel.Singleton.Character;
        }

        // Update is called once per frame
        void Update()
        {
            if (linkedCharacter == null)
                return;

            hpText.text = $"{linkedCharacter.CurrentHp} / {linkedCharacter.MaxHp}";

            if (linkedCharacter.currentWeapon != null)
                weaponText.text = $"{linkedCharacter.currentWeapon.name}";
            else
                weaponText.text = $"Idle";

            if (linkedCharacter.currentWeapon != null)
                bulletText.text = $"{linkedCharacter.currentWeapon.ammo.CurrentAmmo} / {linkedCharacter.currentWeapon.clipSize}";
            else
                bulletText.text = $"None";
        }

        // 추후 뭐 캐릭터가 늘어난다면 이런식으로 동적 연동을 해야할듯?
        public void SetLinkedCharacter(CharacterBase character)
        {
            linkedCharacter = character;
        }
    }
}
