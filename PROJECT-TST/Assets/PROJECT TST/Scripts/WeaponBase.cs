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

        public AmmoBase ammo;
        public Transform firePoint;
        public float fireRate = 0.1f; // 연사 속도
        private float lastFireTime; // 마지막 발사 시간

        public event Func<AmmoBase> SetPlayerAmmo_Event;

        private void Awake()
        {
            
        }

        public void Update()
        {
            if (ammo != null)
                Debug.Log($"{ammo.data.name}");
        }

        public bool Fire()
        {
            if (ammo == null)
                ammo = SetPlayerAmmo_Event?.Invoke();


            if (ammo.CurrentAmmo > 0 && Time.time - lastFireTime >= fireRate)
            {
                lastFireTime = Time.time;
                ammo.CurrentAmmo--;

                GameObject newBullet = Instantiate(ammo.data.AmmoVisualPrefab, firePoint.transform.position, firePoint.transform.rotation);
                newBullet.gameObject.SetActive(true);

                var effect = EffectManager.Singleton.SpawnEffect(ammo.data.AmmoEffectPrefab);
                effect.transform.SetPositionAndRotation(firePoint.position, firePoint.rotation);

                return true;
            }

            return false;
        }

        public void Reload()
        {
            if (ammo == null)
                ammo = SetPlayerAmmo_Event?.Invoke();

            if (ammo.CurrentAmmo <= 0)
            {
                // 노말만 충전
                if (ammo.data.name.Contains("Normal"))
                    ammo.CurrentAmmo = ammo.clipSize;

                // 노말이 아니면 다음 ammo로 넘어감
                ammo = SetPlayerAmmo_Event?.Invoke();
            }
        }
    }
}
