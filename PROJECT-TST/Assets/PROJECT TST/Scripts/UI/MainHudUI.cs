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
        public Image heartImage;

        public Vector2 startRectSize = new Vector2(60.0f, 60.0f);
        public Vector2 endRectSize = new Vector2(120.0f, 120.0f);
        public float targetFillAmount;
        public float pulseSpeed = 2f;

        public TextMeshProUGUI bulletText;
        public TextMeshProUGUI bulletTypeText;
        public CharacterBase linkedCharacter;

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
                        weaponImage.rectTransform.sizeDelta = (rifle.textureRect.size);
                    }

                        break;
                case WeaponType.Pistol:
                    if (AssetManager.Singleton.GetItemIcon("Pistol", out Sprite pistol))
                    {
                        weaponImage.sprite = pistol;
                        weaponImage.rectTransform.sizeDelta = (pistol.textureRect.size);
                    }
                        break;
                case WeaponType.None:
                    if (AssetManager.Singleton.GetItemIcon("Knife", out Sprite knife))
                    {
                        weaponImage.sprite = knife;
                        weaponImage.rectTransform.sizeDelta = (knife.textureRect.size);
                    }
                    break;
            }
        }

        public void Awake()
        {
           
        }

        public void Start()
        {
            GetComponent<Canvas>().worldCamera = CameraSystem.Instance.uiCamera;
        }

        void Update()
        {
            heartImage.fillAmount = Mathf.Lerp(heartImage.fillAmount, targetFillAmount, Time.deltaTime * 10.0f);
        }

        public void SetBulletTextImage()
        {
            if (linkedCharacter.weaponToEquip != null)
            {
                bulletText.text = $"{linkedCharacter.weaponToEquip.WeaponCurrentBulletAmount} / {linkedCharacter.weaponToEquip.MaxBulletAmount}";
                bulletTypeText.text = $"{linkedCharacter.weaponToEquip.GetFirstLoadedBulletName()}";
            }
            else if (linkedCharacter.weaponToEquip == null && linkedCharacter.IsSwitchingWeapon == true)
            {
                bulletText.text = $"1 / 1";
                bulletTypeText.text = $"Knife";
            }
            else if (linkedCharacter.currentWeapon != null)
            {
                bulletText.text = $"{linkedCharacter.currentWeapon.WeaponCurrentBulletAmount} / {linkedCharacter.currentWeapon.MaxBulletAmount}";
                bulletTypeText.text = $"{linkedCharacter.currentWeapon.GetFirstLoadedBulletName()}";
            }
        }

        public void SetHpTextImage()
        {
            hpText.text = $" {linkedCharacter.CurrentHp}";
            targetFillAmount = linkedCharacter.CurrentHp / linkedCharacter.MaxHp;
        }

        // 추후 뭐 캐릭터가 늘어난다면 이런식으로 동적 연동을 해야할듯?
        public void SetLinkedCharacter(CharacterBase character)
        {
            linkedCharacter = character;
        }

        public void SetPulse(float pulseSpeed)
        {
            this.pulseSpeed = pulseSpeed;
        }
    }
}
