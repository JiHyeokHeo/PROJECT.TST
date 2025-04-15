using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace TST
{
    public enum WeaponType
    {
        None = 0,
        Rifle,
        Pistol,
    }

    public class WeaponBase : MonoBehaviour
    {
        public WeaponType WeaponType => weaponType;

        [SerializeField] private WeaponType weaponType;

        public int WeaponCurrentBulletAmount
        {
            get => weaponCurrentBulletAmount;
            set
            {
                weaponCurrentBulletAmount = value;

                if (weaponCurrentBulletAmount >= clipSize)
                {
                    weaponCurrentBulletAmount = clipSize;
                    return;
                }
            }
        }

        public List<AmmoBase> LoadedAmmo { get { return loadedAmmo; } private set { } }

        public int MaxBulletAmount
        {
            get => maxBulletAmount;
            private set { }
        }

        public int clipSize = 30;
        private int weaponCurrentBulletAmount = 0;
        private int maxBulletAmount = 0;

        private AmmoBase ammo;
        public Transform firePoint;
        public float fireRate = 0.1f; // 연사 속도
        private float lastFireTime; // 마지막 발사 시간

        public event Func<AmmoBase, AmmoBase> SetPlayerAmmo_Event;
        private List<AmmoBase> loadedAmmo = new List<AmmoBase>();

        private void Awake()
        {
            if (weaponType == WeaponType.Rifle)
            {
                fireRate = 0.1f;
                clipSize = 30;
            }
            if (weaponType == WeaponType.Pistol)
            {
                fireRate = 0.3f;
                clipSize = 7;
            }
        }

        public void InitializeWeapon(List<AmmoBase> ammos)
        {
            if (ammos.Count > 0)
            {
                for (int i = 0; i < ammos.Count; i++)
                {
                    maxBulletAmount += ammos[i].CurrentBulletAmount;
                }
            }
        }

        public void Update()
        {
            if (ammo != null && ammo.LoadedBulletAmount <= 0)
            {
                ammo = null; 
            }
                

            if (ammo != null)
                Debug.Log($"{ammo.data.name}");

            if (loadedAmmo.Count > 0)
            {
                if (loadedAmmo[0].LoadedBulletAmount <= 0)
                {
                    loadedAmmo.RemoveAt(0);
                }
            }
        }

        public bool Fire()
        {
            if (Time.time - lastFireTime >= fireRate && weaponCurrentBulletAmount > 0)
            {
                lastFireTime = Time.time;
                weaponCurrentBulletAmount--;
                loadedAmmo[0].LoadedBulletAmount--;

                GameObject newBullet = Instantiate(loadedAmmo[0].data.AmmoVisualPrefab, firePoint.transform.position, firePoint.transform.rotation);
                newBullet.gameObject.SetActive(true);

                var effect = EffectManager.Singleton.SpawnEffect(loadedAmmo[0].data.AmmoEffectPrefab);
                effect.transform.SetPositionAndRotation(firePoint.position, firePoint.rotation);

                // 사운드 추가
                if (weaponType == WeaponType.Rifle)
                    SoundManager.Singleton.PlaySFX("Weapon Shot", firePoint.position);
                else if (weaponType == WeaponType.Pistol)
                    SoundManager.Singleton.PlaySFX("Weapon Pistol Shot", firePoint.position);

                return true;
            }

            if (weaponCurrentBulletAmount <= 0)
            {
                if (weaponType == WeaponType.Rifle)
                    SoundManager.Singleton.PlaySFX("Weapon Empty", firePoint.position);
                else if (weaponType == WeaponType.Pistol)
                    SoundManager.Singleton.PlaySFX("Weapon Pistol Empty", firePoint.position);
            }
           
            return false;
        }

        public void Reload()
        {
            // 1. 내가 apc 탄을 10발 남긴 상태에서 장전을 한다? -> weapon에서의 탄창은 곧 currentBulletAmount;
            // 2. apc 탄을 10발 소지한 채로 특수탄을 20발 장전해야한다. -> 그렇다면 특수탄을 일단 장착 한 후
            while (WeaponCurrentBulletAmount < clipSize)
            {
                if (SetPlayerAmmo_Event == null)
                    break;

                // 1. 일단 내가 가지고 있는 특수탄 부터 장착 // 기존에 끼던 ammo 가 동일 하다면 다음 특수탄을 껴야함
                ammo = SetPlayerAmmo_Event?.Invoke(ammo);

                if (ammo == null)
                    break;

                // 2. 장전한 곳으로 전달
                if (loadedAmmo.Exists(x => x.Equals(ammo)) == false)
                    loadedAmmo.Add(ammo);

                // 필요 수량
                int requireAmount = clipSize - WeaponCurrentBulletAmount;

                // 3. 실제로 장전 가능한 양 계산 (필요 수량과 현재 총알 중 더 작은 값)
                int loadAmount = Mathf.Min(requireAmount, ammo.CurrentBulletAmount);

                // 장전
                ammo.LoadedBulletAmount += loadAmount;
                WeaponCurrentBulletAmount += loadAmount;
                maxBulletAmount -= loadAmount;
                ammo.CurrentBulletAmount -= loadAmount;
            }
        }

        public void StartLoadSound()
        {
            if (weaponType == WeaponType.Rifle)
                SoundManager.Singleton.PlaySFX("Weapon Load", firePoint.position);
            else if (weaponType == WeaponType.Pistol)
                SoundManager.Singleton.PlaySFX("Weapon Pistol Load", firePoint.position);
        }

        public void StartUnloadSound()
        {
            if (weaponType == WeaponType.Rifle)
                SoundManager.Singleton.PlaySFX("Weapon Unload", firePoint.position);
            else if (weaponType == WeaponType.Pistol)
                SoundManager.Singleton.PlaySFX("Weapon Pistol Unload", firePoint.position);
        }

        public void AddMaxAmountBullet(int amount)
        {
            maxBulletAmount += amount;
        }

        public String GetFirstLoadedBulletName()
        {
            if (loadedAmmo.Count <= 0)
                return "No Ammo";

            return loadedAmmo[0].data.DataName;
        }
    }
}
