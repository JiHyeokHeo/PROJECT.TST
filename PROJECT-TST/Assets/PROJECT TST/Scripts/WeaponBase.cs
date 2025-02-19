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

        public int CurrentBulletAmount
        {
            get => currentBulletAmount;
            set
            {
                currentBulletAmount = value;

                if (currentBulletAmount >= clipSize)
                {
                    currentBulletAmount = clipSize;
                    return;
                }
            }
        }

        public int MaxBulletAmount
        {
            get => maxBulletAmount;
            private set { }
        }

        public int clipSize = 30;
        private int currentBulletAmount = 0;
        private int maxBulletAmount = 0;

        private AmmoBase ammo;
        public Transform firePoint;
        public float fireRate = 0.1f; // 연사 속도
        private float lastFireTime; // 마지막 발사 시간

        public event Func<AmmoBase> SetPlayerAmmo_Event;
        private List<AmmoBase> loadedAmmo = new List<AmmoBase>();

        private void Awake()
        {
            if (weaponType == WeaponType.Rifle)
                clipSize = 30;
            if (weaponType == WeaponType.Pistol)
                clipSize = 7;
        }

        public void InitializeWeapon(List<AmmoBase> ammos)
        {
            if (ammos.Count > 0)
            {
                for (int i = 0; i < ammos.Count; i++)
                {
                    loadedAmmo.Add(ammos[i]);
                    maxBulletAmount += ammos[i].CurrentAmmo;
                }
            }
        }

        public void Update()
        {
            if (ammo != null)
                //Debug.Log($"{ammo.data.name}");

            if (ammo == null)
                ammo = SetPlayerAmmo_Event?.Invoke();

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
            if (Time.time - lastFireTime >= fireRate && currentBulletAmount > 0)
            {
                lastFireTime = Time.time;
                currentBulletAmount--;
                loadedAmmo[0].LoadedBulletAmount--;

                GameObject newBullet = Instantiate(loadedAmmo[0].data.AmmoVisualPrefab, firePoint.transform.position, firePoint.transform.rotation);
                newBullet.gameObject.SetActive(true);

                var effect = EffectManager.Singleton.SpawnEffect(loadedAmmo[0].data.AmmoEffectPrefab);
                effect.transform.SetPositionAndRotation(firePoint.position, firePoint.rotation);

                return true;
            }
           
            return false;
        }

        public void Reload()
        {
            if (ammo == null)
                ammo = SetPlayerAmmo_Event?.Invoke();

            // 무한 while
            while (CurrentBulletAmount < clipSize)
            {
                if (SetPlayerAmmo_Event == null)
                        break;
                
                // 일단 내가 가지고 있는 특수탄 부터 장착
                ammo = SetPlayerAmmo_Event?.Invoke();

                if (ammo == null)
                    break;

                if (loadedAmmo.Exists(x => x.Equals(ammo)) == false)
                    loadedAmmo.Add(ammo);

                // 총알 빼주기
                int tempAmount = CurrentBulletAmount;
                CurrentBulletAmount += ammo.CurrentAmmo;

                // ammo 데이터에 보유중인 Ammo 감소, reload된 bulletAmount 추가
                tempAmount -= CurrentBulletAmount;
                ammo.CurrentAmmo += tempAmount;
                ammo.LoadedBulletAmount -= tempAmount;
                maxBulletAmount += tempAmount;
            }
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
