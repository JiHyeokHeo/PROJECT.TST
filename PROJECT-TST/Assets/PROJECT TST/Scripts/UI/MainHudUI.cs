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
        public TextMeshProUGUI bulletText;
        public TextMeshProUGUI bulletTypeText;
        public CharacterBase linkedCharacter;

        // Start is called before the first frame update
        void Start()
        {
            CharacterController.Instance.linkedCharacter.onWeaponSwap += SetHud;
            SetHud();
        }

        public void SetHud()
        {
            WeaponType type = WeaponType.None;
            if (CharacterController.Instance.linkedCharacter.weaponToEquip != null)
                type = CharacterController.Instance.linkedCharacter.weaponToEquip.WeaponType;

            switch (type) 
            {
                case WeaponType.Rifle:
                    if (AssetManager.Singleton.GetItemIcon("AR", out Sprite rifle))
                    {
                        weaponImage.sprite = rifle;
                        weaponImage.rectTransform.sizeDelta = (rifle.textureRect.size) / 2f;
                    }

                        break;
                case WeaponType.Pistol:
                    if (AssetManager.Singleton.GetItemIcon("Pistol", out Sprite pistol))
                    {
                        weaponImage.sprite = pistol;
                        weaponImage.rectTransform.sizeDelta = (pistol.textureRect.size) / 2f;
                    }
                        break;
                case WeaponType.None:
                    if (AssetManager.Singleton.GetItemIcon("Knife", out Sprite knife))
                    {
                        weaponImage.sprite = knife;
                        weaponImage.rectTransform.sizeDelta = (knife.textureRect.size) / 2f;
                    }
                    break;
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
                bulletText.text = $"{linkedCharacter.currentWeapon.CurrentBulletAmount} / {linkedCharacter.currentWeapon.MaxBulletAmount}";
                bulletTypeText.text = $"{linkedCharacter.currentWeapon.GetFirstLoadedBulletName()}";
            }
            else
            {
                bulletText.text = $"1 / 1";
                bulletTypeText.text = $"Knife";
            }
        }

        // 추후 뭐 캐릭터가 늘어난다면 이런식으로 동적 연동을 해야할듯?
        public void SetLinkedCharacter(CharacterBase character)
        {
            linkedCharacter = character;
        }
    }
}
