using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TST
{
    public class MainHudUI : UIBase
    {
        public TextMeshProUGUI hpText;
        public Image weaponImage;
        public TextMeshProUGUI weaponText;
        public TextMeshProUGUI bulletText;
        public TextMeshProUGUI bulletTypeText;
        public CharacterBase linkedCharacter;

        // Start is called before the first frame update
        void Start()
        {
            CharacterController.Instance.linkedCharacter.onWeaponSwap += SetHud;
        }

        public void SetHud()
        {
            WeaponType type = CharacterController.Instance.linkedCharacter.currentWeapon.WeaponType;

            switch (type) 
            {

            }
        }

        // Update is called once per frame
        void Update()
        {
            if (linkedCharacter == null)
                return;

            hpText.text = $"{linkedCharacter.CurrentHp} / {linkedCharacter.MaxHp}";

            if (linkedCharacter.currentWeapon != null)
            {
                weaponText.text = $"{linkedCharacter.currentWeapon.name}";
                bulletText.text = $"{linkedCharacter.currentWeapon.CurrentBulletAmount} / {linkedCharacter.currentWeapon.MaxBulletAmount}";
                bulletTypeText.text = $"{linkedCharacter.currentWeapon.GetFirstLoadedBulletName()}";
            }
            else
            {
                weaponText.text = $"Idle";
                bulletText.text = $"None";
                bulletTypeText.text = $"None";
            }
        }

        // 추후 뭐 캐릭터가 늘어난다면 이런식으로 동적 연동을 해야할듯?
        public void SetLinkedCharacter(CharacterBase character)
        {
            linkedCharacter = character;
        }
    }
}
